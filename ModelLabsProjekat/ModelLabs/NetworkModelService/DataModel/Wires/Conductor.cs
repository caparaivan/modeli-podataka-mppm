using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class Conductor : ConductingEquipment
    {
        private float length;

        public Conductor(long globalId) : base(globalId)
        {
        }

        public float Length
        {
            get { return length; }
            set { length = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                return ((Conductor)obj).length == length;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ length.GetHashCode();
                return hash;
            }
        }

        public override bool HasProperty(ModelCode property)
        {
            if (property == ModelCode.CONDUCTOR_LENGTH)
            {
                return true;
            }

            return base.HasProperty(property);
        }

        public override void GetProperty(Property property)
        {
            if (property.Id == ModelCode.CONDUCTOR_LENGTH)
            {
                property.SetValue(length);
            }
            else
            {
                base.GetProperty(property);
            }
        }

        public override void SetProperty(Property property)
        {
            if (property.Id == ModelCode.CONDUCTOR_LENGTH)
            {
                length = property.AsFloat();
            }
            else
            {
                base.SetProperty(property);
            }
        }
    }
}