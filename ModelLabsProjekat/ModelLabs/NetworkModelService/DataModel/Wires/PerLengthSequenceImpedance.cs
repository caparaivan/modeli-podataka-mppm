using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class PerLengthSequenceImpedance : PerLengthImpedance
    {
        private float r;
        private float r0;
        private float x;
        private float x0;
        private float bch;
        private float b0ch;
        private float gch;
        private float g0ch;

        public PerLengthSequenceImpedance(long globalId) : base(globalId)
        {
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                PerLengthSequenceImpedance xObj = (PerLengthSequenceImpedance)obj;
                return xObj.r == r && xObj.r0 == r0 && xObj.x == x && xObj.x0 == x0 &&
                       xObj.bch == bch && xObj.b0ch == b0ch && xObj.gch == gch && xObj.g0ch == g0ch;
            }

            return false;
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_R:
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_R0:
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_X:
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_X0:
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_BCH:
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_B0CH:
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_GCH:
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_G0CH:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_R: property.SetValue(r); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_R0: property.SetValue(r0); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_X: property.SetValue(x); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_X0: property.SetValue(x0); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_BCH: property.SetValue(bch); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_B0CH: property.SetValue(b0ch); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_GCH: property.SetValue(gch); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_G0CH: property.SetValue(g0ch); break;
                default: base.GetProperty(property); break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_R: r = property.AsFloat(); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_R0: r0 = property.AsFloat(); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_X: x = property.AsFloat(); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_X0: x0 = property.AsFloat(); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_BCH: bch = property.AsFloat(); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_B0CH: b0ch = property.AsFloat(); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_GCH: gch = property.AsFloat(); break;
                case ModelCode.PERLENGTHSEQUENCEIMPEDANCE_G0CH: g0ch = property.AsFloat(); break;
                default: base.SetProperty(property); break;
            }
        }
    }
}