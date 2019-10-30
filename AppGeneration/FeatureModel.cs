using System.Collections.Generic;

namespace AppGeneration
{
    public class FeatureModel
    {
        public readonly Dictionary<string, bool> Features =  new Dictionary<string, bool>();


        public FeatureModel()
        {
        }

        public void addFeatureWithSelectFlag(string name, bool isSelected)
        {
            Features.Add(name, isSelected);
        }
    }
}