using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using PharmaACE.Utility;

namespace PharmaACE.NLP.Framework
{
    public class SentenceFragment : ICloneable
    {
        public List<RecognizedEntity> RecognizedEntities { get; set; }

        public object Clone()
        {
            SentenceFragment fragmentClone = this.MemberwiseClone() as SentenceFragment;
            fragmentClone.RecognizedEntities = new List<RecognizedEntity>();
            if (fragmentClone != null) {
                foreach (var ner in this.RecognizedEntities)
                {
                    var nerClone = ner.Clone() as RecognizedEntity;
                    if(nerClone != null)
                        fragmentClone.RecognizedEntities.Add(nerClone);
                }
            }

            return fragmentClone;
        }

        /// <summary>
        /// gets entity value by field name from a sentence fragment object
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns>entity value as object, null if not found</returns>
        public object GetEntityValueByFieldName(string entityName)
        {
            return this.RecognizedEntities.
            Where(re => String.Compare(re.Entity.FieldName.SafeTrim(), entityName.SafeTrim(), true) == 0).
            Select(re => re.Value).
            FirstOrDefault();
        }

        /// <summary>
        /// gets entity value by domain name from a sentence fragment object
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns>entity value as object, null if not found</returns>
        public object GetEntityValueByDomainName(string entityName)
        {
            return this.RecognizedEntities.
            Where(re => String.Compare(re.Entity.DomainName.SafeTrim(), entityName.SafeTrim(), true) == 0).
            Select(re => re.Value).
            FirstOrDefault();
        }

        /// <summary>
        /// gets entity value, as it appears in the question, by domain name from a sentence fragment object
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns>entity recognized value as object, null if not found</returns>
        public object GetEntityRecognizedValueByFieldName(string entityName)
        {
            return this.RecognizedEntities.
            Where(re => String.Compare(re.Entity.FieldName.SafeTrim(), entityName.SafeTrim(), true) == 0).
            Select(re => re.RecognizedValue).
            FirstOrDefault();
        }

        /// <summary>
        /// gets entity value, as it appears in the question, by domain name from a sentence fragment object
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns>entity recognized value as object, null if not found</returns>
        public object GetEntityRecognizedValueByDomainName(string entityName)
        {
            return this.RecognizedEntities.
            Where(re => String.Compare(re.Entity.DomainName.SafeTrim(), entityName.SafeTrim(), true) == 0).
            Select(re => re.RecognizedValue).
            FirstOrDefault();
        }

        public bool ReplaceNERs(SentenceFragment fragmentFrom)
        {
            bool success = false;
            try
            {
                foreach (var re in fragmentFrom.RecognizedEntities)
                {
                    bool replaced = false;
                    for (int i = 0; i < this.RecognizedEntities.Count; i++)
                    {
                        if (this.RecognizedEntities[i].IsSameDimension(re))
                        {
                            this.RecognizedEntities[i] = re;
                            replaced = true;
                            success = true;
                        }
                    }
                    if (!replaced)
                        this.RecognizedEntities.Add(re);
                }
            }
            catch(Exception ex)
            {
                //could not replace or add
            }

            return success;
        }

        public bool IsSubsetOf(SentenceFragment fragment)
        {
            if (fragment == null || fragment.RecognizedEntities == null || this.RecognizedEntities == null)
                return false;
            foreach (var ner in this.RecognizedEntities)
            {
                if (!fragment.RecognizedEntities.Contains(ner, new NERComparer()))
                    return false;
            }

            return true;
        }

        public bool IsEquivalent(SentenceFragment fragment)
        {
            if (fragment == null || fragment.RecognizedEntities == null || this.RecognizedEntities == null)
                return false;
            foreach (var ner in fragment.RecognizedEntities)
            {
                if (!this.RecognizedEntities.Contains(ner, new NERComparer()))
                    return false;
            }

            return true;
        }

        public bool IsEquivalent(SentenceFragment fragment, List<string> dimensionNames)
        {
            if (fragment == null || fragment.RecognizedEntities == null || this.RecognizedEntities == null)
                return false;
            foreach (var ner in fragment.RecognizedEntities)
            {
                if (dimensionNames.Contains(ner.Entity.DomainName, StringComparer.InvariantCultureIgnoreCase) && 
                    !this.RecognizedEntities.Contains(ner, new NERComparer()))
                    return false;
            }

            return true;
        }

        public bool IsNEDEquivalent(SentenceFragment fragment)
        {
            if (fragment == null || fragment.RecognizedEntities == null || this.RecognizedEntities == null)
                return false;
            int nedCount = 0;
            //match 1st against 2nd fragment
            foreach (var ner in fragment.RecognizedEntities)
            {
                if (ner.Entity is NED)
                {
                    nedCount++;
                    bool found = false;
                    foreach (var re in this.RecognizedEntities)
                    {
                        if (re.Entity is NED && !new NERComparer().Equals(ner, re))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        return false;
                }
            }
            //if mismatch in NED count they are obviously not ned-equivalent - that completes matching 2nd against 1st fragment as well
            if (this.RecognizedEntities.Count(re => re.Entity is NED) > nedCount)
                return false;

            return true;
        }

        public bool AddOrReplaceNERs(List<RecognizedEntity> ners)
        {
            bool success = false;
            try
            {
                if (this.RecognizedEntities.AnyOrNotNull())
                {
                    foreach (var re in ners)
                    {
                        bool replaced = false;
                        for (int i = 0; i < this.RecognizedEntities.Count; i++)
                        {
                            if (this.RecognizedEntities[i].IsSameDimension(re))
                            {
                                this.RecognizedEntities[i] = re;
                                replaced = true;
                                success = true;
                            }
                        }
                        if (!replaced)
                            this.RecognizedEntities.Add(re);
                    }
                }
                else
                    this.RecognizedEntities = ners;
            }
            catch (Exception ex)
            {
                //could not replace or add
            }

            return success;
        }

        public bool AddOrReplaceNERs(SentenceFragment fragmentFrom)
        {
            bool success = false;
            try
            {
                foreach (var re in fragmentFrom.RecognizedEntities)
                {
                    bool replaced = false;
                    for (int i = 0; i < this.RecognizedEntities.Count; i++)
                    {
                        if (this.RecognizedEntities[i].IsSameDimension(re))
                        {
                            this.RecognizedEntities[i] = re;
                            replaced = true;
                            success = true;
                        }
                    }
                    if (!replaced)
                        this.RecognizedEntities.Add(re);
                }
            }
            catch (Exception ex)
            {
                //could not replace or add
            }

            return success;
        }

        public bool HasMeasure()
        {
            return this.RecognizedEntities.Any(re => re.IsMeasure);
        }
    }

    public class RecognizedEntity : ICloneable
    {
        /// <summary>
        /// field name
        /// </summary>
        public Dimension Entity { get; set; }
        /// <summary>
        /// field value
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// name recognized in sentence
        /// </summary>
        public string RecognizedName { get; set; }
        /// <summary>
        /// value recognized in sentence
        /// </summary>
        public object RecognizedValue { get; set; }
        /// <summary>
        /// field appearance/preference order
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// occurrence index inside a sentence
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Retaion between Entity Name/RecognizedName and Entity Value/RecognizedValue
        /// </summary>
        public NERClause Clause { get; set; }

        /// <summary>
        /// Natural language interpretation of the clause assigned to the entity
        /// </summary>
        public string NLClause {
            get
            {
                return String.Format(" {0} {2} '{1}'", Entity.DomainName,
                                Value.ToString().Replace(" ", Util.PHRASE_MAKER).Replace("'", String.Empty).Replace("\"", String.Empty),
                                Clause.ToString()).
                            Replace("$", "$$");
            }
        }

        /// <summary>
        /// checks for overlapping NERs
        /// </summary>
        /// <param name="ner"></param>
        /// <returns></returns>
        public bool Overlaps(RecognizedEntity ner)
        {
            string overlapper = null;
            string overlapped = null;
            if (IsNamedEntity)
            {
                overlapper = this?.RecognizedValue?.ToString();
                overlapped = ner?.RecognizedValue?.ToString();
            }
            if(IsMeasure)
            {
                overlapper = this?.RecognizedName?.ToString();
                overlapped = ner?.RecognizedName?.ToString();
            }

            if (String.IsNullOrEmpty(overlapper) || String.IsNullOrEmpty(overlapped))
                return false;

            return this.Index <= ner.Index &&
                   this.Index + overlapper.Length >= ner.Index + overlapped.Length &&
                   overlapper.Contains(overlapped);
        }

        /// <summary>
        /// detects type of dimension
        /// </summary>
        /// <returns>true if recognized entity is NED, otherwise false</returns>
        public bool IsNamedEntity
        {
            get
            {
                return Entity as NED != null;
            }
        }

        /// <summary>
        /// detects type of dimension
        /// </summary>
        /// <returns>true if recognized entity is Measure, otherwise false</returns>
        public bool IsMeasure
        {
            get
            {
                return Entity as Measure != null;
            }
        }

        public bool IsPlural
        {
            get
            {
                CultureInfo ci = new CultureInfo("en-us");
                PluralizationService ps =
                  PluralizationService.CreateService(ci);
                if(this.IsNamedEntity)
                    return ps.IsPlural(this.RecognizedValue.ToString());
                else if(this.IsMeasure)
                    return ps.IsPlural(this.RecognizedName);

                return false;
            }
        }

        public void Pluralize()
        {
            CultureInfo ci = new CultureInfo("en-us");
            PluralizationService ps =
              PluralizationService.CreateService(ci);
            if (this.IsNamedEntity)
                this.RecognizedValue = ps.Pluralize(this.RecognizedValue.ToString());
            else if (this.IsMeasure)
                this.RecognizedName = ps.Pluralize(this.RecognizedName);
        }

        public void Singularize()
        {
            CultureInfo ci = new CultureInfo("en-us");
            PluralizationService ps =
              PluralizationService.CreateService(ci);
            if (this.IsNamedEntity)
                this.RecognizedValue = ps.Singularize(this.RecognizedValue.ToString());
            else if (this.IsMeasure)
                this.RecognizedName = ps.Singularize(this.RecognizedName);
        }

        public bool IsSameDimension(RecognizedEntity ner)
        {
            return String.Compare(this.Entity.DomainName, ner.Entity.DomainName, true) == 0;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public enum NERClause
    {
        EQUAL,
        NOTEQUAL,
        GREATER,
        LESSER,
        GREATEREQUAL,
        LESSEREQUAL
    }

    public class NERComparer : IEqualityComparer<RecognizedEntity>
    {
        public bool Equals(RecognizedEntity x, RecognizedEntity y)
        {
            return String.Compare(x.Entity.DomainName, y.Entity.DomainName, true) == 0 && String.Compare(x.Value.ToString(), y.Value.ToString(), true) == 0;
        }

        public int GetHashCode(RecognizedEntity obj)
        {
            return (obj.Entity.DomainName + obj.Value.ToString()).GetHashCode();
        }
    }
}