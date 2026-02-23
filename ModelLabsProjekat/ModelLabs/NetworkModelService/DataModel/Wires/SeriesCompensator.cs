using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class SeriesCompensator : ConductingEquipment
    {
        private float r;
        private float r0;
        private float x;
        private float x0;

        public SeriesCompensator(long globalId) : base(globalId)
        {
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                SeriesCompensator xObj = (SeriesCompensator)obj;
                return xObj.r == r && xObj.r0 == r0 && xObj.x == x && xObj.x0 == x0;
            }

            return false;
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.SERIESCOMPENSATOR_R:
                case ModelCode.SERIESCOMPENSATOR_R0:
                case ModelCode.SERIESCOMPENSATOR_X:
                case ModelCode.SERIESCOMPENSATOR_X0:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SERIESCOMPENSATOR_R: property.SetValue(r); break;
                case ModelCode.SERIESCOMPENSATOR_R0: property.SetValue(r0); break;
                case ModelCode.SERIESCOMPENSATOR_X: property.SetValue(x); break;
                case ModelCode.SERIESCOMPENSATOR_X0: property.SetValue(x0); break;
                default: base.GetProperty(property); break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SERIESCOMPENSATOR_R: r = property.AsFloat(); break;
                case ModelCode.SERIESCOMPENSATOR_R0: r0 = property.AsFloat(); break;
                case ModelCode.SERIESCOMPENSATOR_X: x = property.AsFloat(); break;
                case ModelCode.SERIESCOMPENSATOR_X0: x0 = property.AsFloat(); break;
                default: base.SetProperty(property); break;
            }
        }
    }
}