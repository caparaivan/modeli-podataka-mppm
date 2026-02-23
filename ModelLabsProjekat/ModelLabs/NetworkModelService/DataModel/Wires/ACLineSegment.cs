using System;
using System.Collections.Generic;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class ACLineSegment : Conductor
    {
        private float r;
        private float r0;
        private float x;
        private float x0;
        private float bch;
        private float b0ch;
        private float gch;
        private float g0ch;
        private long perLengthImpedance;

        public ACLineSegment(long globalId) : base(globalId)
        {
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                ACLineSegment xObj = (ACLineSegment)obj;
                return xObj.r == r && xObj.r0 == r0 && xObj.x == x && xObj.x0 == x0 && xObj.bch == bch &&
                       xObj.b0ch == b0ch && xObj.gch == gch && xObj.g0ch == g0ch && xObj.perLengthImpedance == perLengthImpedance;
            }

            return false;
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.ACLINESEGMENT_R:
                case ModelCode.ACLINESEGMENT_R0:
                case ModelCode.ACLINESEGMENT_X:
                case ModelCode.ACLINESEGMENT_X0:
                case ModelCode.ACLINESEGMENT_BCH:
                case ModelCode.ACLINESEGMENT_B0CH:
                case ModelCode.ACLINESEGMENT_GCH:
                case ModelCode.ACLINESEGMENT_G0CH:
                case ModelCode.ACLINESEGMENT_PERLENGTHIMP:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ACLINESEGMENT_R: property.SetValue(r); break;
                case ModelCode.ACLINESEGMENT_R0: property.SetValue(r0); break;
                case ModelCode.ACLINESEGMENT_X: property.SetValue(x); break;
                case ModelCode.ACLINESEGMENT_X0: property.SetValue(x0); break;
                case ModelCode.ACLINESEGMENT_BCH: property.SetValue(bch); break;
                case ModelCode.ACLINESEGMENT_B0CH: property.SetValue(b0ch); break;
                case ModelCode.ACLINESEGMENT_GCH: property.SetValue(gch); break;
                case ModelCode.ACLINESEGMENT_G0CH: property.SetValue(g0ch); break;
                case ModelCode.ACLINESEGMENT_PERLENGTHIMP: property.SetValue(perLengthImpedance); break;
                default: base.GetProperty(property); break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ACLINESEGMENT_R: r = property.AsFloat(); break;
                case ModelCode.ACLINESEGMENT_R0: r0 = property.AsFloat(); break;
                case ModelCode.ACLINESEGMENT_X: x = property.AsFloat(); break;
                case ModelCode.ACLINESEGMENT_X0: x0 = property.AsFloat(); break;
                case ModelCode.ACLINESEGMENT_BCH: bch = property.AsFloat(); break;
                case ModelCode.ACLINESEGMENT_B0CH: b0ch = property.AsFloat(); break;
                case ModelCode.ACLINESEGMENT_GCH: gch = property.AsFloat(); break;
                case ModelCode.ACLINESEGMENT_G0CH: g0ch = property.AsFloat(); break;
                case ModelCode.ACLINESEGMENT_PERLENGTHIMP: perLengthImpedance = property.AsReference(); break;
                default: base.SetProperty(property); break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (perLengthImpedance != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.ACLINESEGMENT_PERLENGTHIMP] = new List<long> { perLengthImpedance };
            }

            base.GetReferences(references, refType);
        }
    }
}