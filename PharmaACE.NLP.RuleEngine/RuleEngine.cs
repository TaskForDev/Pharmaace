using System;
using System.Collections.Generic;
using System.Linq;

namespace PharmaACE.NLP.Framework
{
    public abstract class RuleEngineBase
    {
        /// <summary>
        /// rule engine is designed for unique measure per question
        /// </summary>
        protected RecognizedEntity identifiedMeasure;

        /// <summary>
        /// start and end datetime under scope
        /// </summary>
        public DateRange TimeSeriesBoundary { get; set; }

        /// <summary>
        /// gets all named entities
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetAllEntityPhrases();

        /// <summary>
        /// recognises entities in a question
        /// </summary>
        /// <param name="question"></param>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        public abstract RecognitionOutput ResolveDimensions(NaturalLanguageQuestion question, Database databaseObject);

        /// <summary>
        /// decides the type of visualization based on given data
        /// </summary>
        /// <param name="dataSlices"></param>
        /// <returns></returns>
        public abstract Visualization ResolveChartTypeFromActualData(List<SentenceFragment> dataSlices);

        /// <summary>
        /// gets the order in which the entities need to be sorted
        /// </summary>
        /// <returns></returns>
        public abstract string GetDefaultDimensionOrder();

        /// <summary>
        /// table or view or the catalog tied to the rule engine
        /// </summary>
        public abstract string DbEntity { get; }
        
        /// <summary>
        /// named entities possible under the rule engine
        /// </summary>
        protected List<NED> NamedEntityDimensions { get; set; }
        
        /// <summary>
        /// normalizes/flattens a group of sentence fragments to have indepently identifiable/parsable fragments
        /// </summary>
        /// <param name="sentenceFragments">group of fragments (each fragment being composed of recognizable entities) to be modified</param>
        protected void NormalizeEntities(ref List<SentenceFragment> sentenceFragments, int mainFragmentIndex = -1)
        {
            //force normalization for single fragment
            if (sentenceFragments.Count == 1)
                mainFragmentIndex = -1;
            List<Dimension> allDimensions = NamedEntityDimensions.
                SelectMany(ned => ned.Components ?? new List<NED> { ned }).
                Cast<Dimension>().
                ToList();
            allDimensions.Add(identifiedMeasure.Entity);
            for (int dimOrder = 0; dimOrder < allDimensions.Count; dimOrder++)
            {
                var dimension = allDimensions[dimOrder];
                //check if dimension is found for each fragment
                var ners = sentenceFragments.
                    Select(sf => sf.RecognizedEntities.
                    Where(ner => String.Compare(ner.Entity.DomainName, dimension.DomainName, true) == 0).
                    FirstOrDefault()).
                    ToList();
                ners.Where(ner => ner != null).ToList().ForEach(ner => { ner.Order = dimOrder; });
                int numberOfFragmentsWithDim = ners.Count(dim => dim != null);

                //if dimension is present in each fragment, all good
                //if dimension is present in 1 fragment out of many, apply same root to other fragments assuming all are meant for same dimension
                //if dimension is present in multiple fragments out of many, fragments without root is assumed to follow the 1st backtracked predecessor dimension
                //if dimension is root but not present at all, return invalid
                if (numberOfFragmentsWithDim < sentenceFragments.Count)
                {
                    //consider default value
                    string defaultDimVal = dimension.Default == null ? String.Empty : dimension.Default.ToString();
                    if (numberOfFragmentsWithDim == 0)
                    {
                        if (!String.IsNullOrEmpty(defaultDimVal))
                        {
                            for (int i = 0; i < ners.Count; i++)
                            {
                                var clonedFragment = sentenceFragments[i].Clone() as SentenceFragment;
                                /*clonedFragment.*/
                                sentenceFragments[i].RecognizedEntities.Insert(0, new RecognizedEntity
                                    {
                                        Entity = new NED { DomainName = dimension.DomainName },
                                        Value = defaultDimVal,
                                        RecognizedValue = defaultDimVal,
                                        Order = dimOrder
                                    });
                                //if (!(dimension is NED) || sentenceFragments.Count(sf => sf.IsNEDEquivalent(clonedFragment)) == 0)
                                //{
                                //    sentenceFragments[i] = clonedFragment;
                                //}
                            }
                        }
                    }
                    else
                    {
                        var ner = ners.First(root => root != null); //1st dimension encountered
                        for (int i = 0; i < ners.Count; i++)
                        {
                            //consider missing NER value
                            if (ners[i] == null)
                            {
                                Dimension dim = null;
                                if (ner.IsMeasure)
                                {
                                    if (identifiedMeasure.Entity.DomainName.Equals(dimension.DomainName, StringComparison.InvariantCultureIgnoreCase))
                                        dim = identifiedMeasure.Entity;
                                }
                                else
                                    dim = new NED { DomainName = dimension.DomainName };
                                if (mainFragmentIndex != i)
                                {
                                    var clonedFragment = sentenceFragments[i].Clone() as SentenceFragment;
                                    /*clonedFragment.*/
                                    sentenceFragments[i].RecognizedEntities.Insert(0, new RecognizedEntity
                                    {
                                        Entity = dim,
                                        RecognizedName = ner.RecognizedName,
                                        Value = ner.Value,
                                        RecognizedValue = ner.RecognizedValue,
                                        Order = dimOrder
                                    });
                                }
                                //if (!(dimension is NED) || sentenceFragments.Count(sf => sf.IsNEDEquivalent(clonedFragment)) == 0)
                                //{
                                //    sentenceFragments[i] = clonedFragment;
                                //}
                            }
                            else
                                ner = ners[i]; //update ner that will be applied to the next fragment-without-root encountered
                        }
                    }
                }
            }

            //sort NERs according to dimension hierarchy level
            sentenceFragments.ForEach(sf => sf.RecognizedEntities = sf.RecognizedEntities?.OrderBy(ner => ner.Order).ToList());
        }

        /// <summary>
        /// domain rules applied to farther update the recognized fragments
        /// </summary>
        /// <param name="sentenceFragments"></param>
        /// <returns></returns>
        protected abstract RuleValidationResult ApplyRules(ref List<SentenceFragment> sentenceFragments);

    }
}
