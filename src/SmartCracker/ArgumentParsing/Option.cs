using System;

namespace SmartCracker.ArgumentParsing
{
    public class Option
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public string Explaination { get; set; }
        public string ShortArgument { get; set; }
        public string LongArgument { get; set; }
        public string DefaultValue { get; set; }
        public dynamic GivenInput { get; set; }
        public OptionTypeEnum Type { get; set; }
        private Action<Option> ValidationAction { get; set; }

        public Option(string optionName, string explaination, OptionTypeEnum optionType, string shortArgument = "", string longArgument = "", string defaultValue = "", bool required = false, Action<Option> validationAction = null)
        {
            Name = optionName;
            Explaination = explaination.Trim();
            Type = optionType;
            ShortArgument = shortArgument.Trim();
            LongArgument = longArgument.Trim();
            Required = required;
            DefaultValue = defaultValue.Trim();
            ValidationAction = validationAction;
        }

        public void ValidateAction()
        {
            if (ValidationAction != null)
            {
                ValidationAction(this);
            }
        }
    }
}
