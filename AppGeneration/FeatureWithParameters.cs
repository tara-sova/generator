namespace AppGeneration.AppForGeneration
{
    public class FeatureWithParameters
    {
        public string featureName { get; }
        public string abstractFeatureName { get; }
        public string longClickTO { get; }
        public string longClickFROM { get; }
        public string clickTO { get; }
        public string clickFROM { get; }
        public bool required { get; }
        public FeatureWithParameters(string featureName = null,
                                     string abstractFeatureName = null,
                                     bool required = false,
                                     string longClickTO = null, 
                                     string longClickFROM = null, 
                                     string clickTO = null, 
                                     string clickFROM = null)
        {
            this.featureName = featureName;
            this.abstractFeatureName = abstractFeatureName;
            this.required = false;
            this.longClickTO = longClickTO;
            this.longClickFROM = longClickFROM;
            this.clickTO = clickTO;
            this.clickFROM = clickFROM;
        }
    }
}