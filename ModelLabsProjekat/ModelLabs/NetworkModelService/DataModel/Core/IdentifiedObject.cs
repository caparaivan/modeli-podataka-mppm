using System;
using System.Collections.Generic;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel
{
    public class IdentifiedObject
    {
        private long globalId;
        private string mRID = string.Empty;
        private string name = string.Empty;
        private string description = string.Empty;

        public IdentifiedObject(long globalId)
        {
            this.globalId = globalId;
        }

        public long GlobalId
        {
            get { return globalId; }
            set { globalId = value; }
        }

        public string MRID
        {
            get { return mRID; }
            set { mRID = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IdentifiedObject x))
            {
                return false;
            }

            return x.globalId == globalId && x.mRID == mRID && x.name == name && x.description == description;
        }

        public override int GetHashCode()
        {
            return globalId.GetHashCode();
        }

        public virtual bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.IDOBJ_GID:
                case ModelCode.IDOBJ_MRID:
                case ModelCode.IDOBJ_NAME:
                case ModelCode.IDOBJ_DESCRIPTION:
                    return true;
                default:
                    return false;
            }
        }

        public virtual void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.IDOBJ_GID:
                    property.SetValue(globalId);
                    break;
                case ModelCode.IDOBJ_MRID:
                    property.SetValue(mRID);
                    break;
                case ModelCode.IDOBJ_NAME:
                    property.SetValue(name);
                    break;
                case ModelCode.IDOBJ_DESCRIPTION:
                    property.SetValue(description);
                    break;
                default:
                    throw new Exception(string.Format("Property {0} not found on {1}.", property.Id, GetType().Name));
            }
        }

        public Property GetProperty(ModelCode property)
        {
            Property p = new Property(property);
            GetProperty(p);
            return p;
        }

        public virtual void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.IDOBJ_MRID:
                    mRID = property.AsString();
                    break;
                case ModelCode.IDOBJ_NAME:
                    name = property.AsString();
                    break;
                case ModelCode.IDOBJ_DESCRIPTION:
                    description = property.AsString();
                    break;
                default:
                    throw new Exception(string.Format("Property {0} not settable on {1}.", property.Id, GetType().Name));
            }
        }

        public virtual bool IsReferenced
        {
            get { return false; }
        }

        public virtual void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
        }

        public virtual void AddReference(ModelCode referenceId, long globalId)
        {
            CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) cannot add reference {1} from 0x{2:x16}.", this.globalId, referenceId, globalId);
        }

        public virtual void RemoveReference(ModelCode referenceId, long globalId)
        {
            CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) cannot remove reference {1} from 0x{2:x16}.", this.globalId, referenceId, globalId);
        }
    }
}