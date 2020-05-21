using MomentSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.DateTimeParser
{
    public class ParsedComponents : ICloneable
    {
        internal Dictionary<string, int> knownValues;
        internal Dictionary<string, int> impliedValues;
        internal Moment Moment {
            get
            {
                var dateMoment = new Moment();

                dateMoment.Year = this.GetValue("year");
                dateMoment.Month = this.GetValue("month");
                dateMoment.Day = this.GetValue("day");
                dateMoment.Hour = this.GetValue("hour");
                dateMoment.Minute = this.GetValue("minute");
                dateMoment.Second = this.GetValue("second");
                dateMoment.Millisecond = this.GetValue("millisecond");

                //TODO: take care of the following commented part
                // Javascript Date Object return minus timezone offset
                //var currentTimezoneOffset = TimeZone.CurrentTimeZone.GetUtcOffset(dateMoment.DateTime());
                //var targetTimezoneOffset = this.GetValue("timezoneOffset") != -1 ? this.GetValue("timezoneOffset") : currentTimezoneOffset;
                //var adjustTimezoneOffset = targetTimezoneOffset - currentTimezoneOffset;
                //dateMoment.add(-adjustTimezoneOffset, "minutes");

                return dateMoment;
            }
        }

        public DateTime Date {
            get
            {
                return this.Moment.DateTime();
            }
        }

        public ParsedComponents()
        {
            knownValues = new Dictionary<string, int>();
            impliedValues = new Dictionary<string, int>();
        }

        public ParsedComponents(Dictionary<string, int> components, DateTime? reference)
        {
            knownValues = new Dictionary<string, int>();
            impliedValues = new Dictionary<string, int>();

            if (components != null)
            {
                foreach (var kvp in components)
                {
                    if(!knownValues.ContainsKey(kvp.Key))
                        knownValues.Add(kvp.Key, components[kvp.Key]);
                }
            }

            if (reference.HasValue)
            {
                Moment m = Parser.GetMoment(reference.Value);
                Imply("day", m.Day);
                Imply("month", m.Month);
                Imply("year", m.Year);
            }

            Imply("hour", 0);
            Imply("minute", 0);
            Imply("second", 0);
            Imply("millisecond", 0);
        }

        internal void Imply(string component, int value)
        {
            if (knownValues.ContainsKey(component))
                return;
            impliedValues[component] = value;
        }
        
        public bool IsPossibleDate
        {
            get
            {
                var dateMoment = Moment;
                //TODO: take care of the following commented part
                //if(IsCertain("timezoneOffset"))
                //    dateMoment.utcOffset(this.get('timezoneOffset'))

                if (dateMoment.Year != GetValue("year"))
                    return false;
                if (dateMoment.Month != GetValue("month") - 1)
                    return false;
                if (dateMoment.Day != GetValue("day"))
                    return false;
                if (dateMoment.Hour != GetValue("hour"))
                    return false;
                if (dateMoment.Minute != GetValue("minute"))
                    return false;

                return true;
            }
        }

        internal bool IsCertain(string component)
        {
            return knownValues.ContainsKey(component);
        }

        internal void Assign(string component, int value)
        {
            knownValues[component] = value;
            if(impliedValues.ContainsKey(component))
                impliedValues.Remove(component);
        }

        internal int GetValue(string component)
        {
            if (knownValues.ContainsKey(component))
                return knownValues[component];
            if (impliedValues.ContainsKey(component))
                return impliedValues[component];

            return -1;
        }

        public object Clone()
        {
            ParsedComponents pc = (ParsedComponents)this.MemberwiseClone();
            pc.knownValues = new Dictionary<string, int>(this.knownValues);
            pc.impliedValues = new Dictionary<string, int>(this.impliedValues);

            return pc;
        }
    }
}
