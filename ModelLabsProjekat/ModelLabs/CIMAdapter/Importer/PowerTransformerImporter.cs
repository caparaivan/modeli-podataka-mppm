using System;
using System.Collections.Generic;
using CIM.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
	/// <summary>
	/// PowerTransformerImporter
	/// </summary>
	public class PowerTransformerImporter
	{
		/// <summary> Singleton </summary>
		private static PowerTransformerImporter ptImporter = null;
		private static object singletoneLock = new object();

		private ConcreteModel concreteModel;
		private Delta delta;
		private ImportHelper importHelper;
		private TransformAndLoadReport report;


		#region Properties
		public static PowerTransformerImporter Instance
		{
			get
			{
				if (ptImporter == null)
				{
					lock (singletoneLock)
					{
						if (ptImporter == null)
						{
							ptImporter = new PowerTransformerImporter();
							ptImporter.Reset();
						}
					}
				}
				return ptImporter;
			}
		}

		public Delta NMSDelta
		{
			get 
			{
				return delta;
			}
		}
		#endregion Properties


		public void Reset()
		{
			concreteModel = null;
			delta = new Delta();
			importHelper = new ImportHelper();
			report = null;
		}

		public TransformAndLoadReport CreateNMSDelta(ConcreteModel cimConcreteModel)
		{
            LogManager.Log("Importing profile elements...", LogLevel.Info); report = new TransformAndLoadReport();
			concreteModel = cimConcreteModel;
			delta.ClearDeltaOperations();

			if ((concreteModel != null) && (concreteModel.ModelMap != null))
			{
				try
				{
					// convert into DMS elements
					ConvertModelAndPopulateDelta();
				}
				catch (Exception ex)
				{
					string message = string.Format("{0} - ERROR in data import - {1}", DateTime.Now, ex.Message);
					LogManager.Log(message);
					report.Report.AppendLine(ex.Message);
					report.Success = false;
				}
			}
            LogManager.Log("Importing profile elements - END.", LogLevel.Info); return report;
		}

		/// <summary>
		/// Method performs conversion of network elements from CIM based concrete model into DMS model.
		/// </summary>
		private void ConvertModelAndPopulateDelta()
		{
			LogManager.Log("Loading elements and creating delta...", LogLevel.Info);

            //// import all concrete model types (DMSType enum)
            //ImportBaseVoltages();
            //ImportLocations();
            //ImportPowerTransformers();
            //ImportTransformerWindings();
            //ImportWindingTests();

            ImportConnectivityNodes();
            ImportPerLengthSequenceImpedances();
            ImportSeriesCompensators();
            ImportDCLineSegments();
            ImportACLineSegments();
            ImportTerminals();

            LogManager.Log("Loading elements and creating delta completed.", LogLevel.Info);
		}

        #region Import
        private void ImportConnectivityNodes()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.ConnectivityNode");
            if (cimObjects == null)
            {
                return;
            }
			foreach (KeyValuePair<string, object> cimObjectPair in cimObjects)
			{
				FTN.ConnectivityNode cimObject = cimObjectPair.Value as FTN.ConnectivityNode;

				ResourceDescription rd = CreateConnectivityNodeResourceDescription(cimObject);
				if (rd != null)
				{
					delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
					report.Report.Append("ConnectivityNode ID = ").Append(cimObject.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
				}
				else
				{
					report.Report.Append("ConnectivityNode ID = ").Append(cimObject.ID).AppendLine(" FAILED to be converted");
				}
			}
                report.Report.AppendLine();
		}

        private ResourceDescription CreateConnectivityNodeResourceDescription(FTN.ConnectivityNode cimConnectivityNode)
        {
			ResourceDescription rd = null;
			if (cimConnectivityNode != null)			{
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.CONNECTIVITYNODE, importHelper.CheckOutIndexForDMSType(DMSType.CONNECTIVITYNODE));
				rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimConnectivityNode.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateConnectivityNodeProperties(cimConnectivityNode, rd);
            }
			return rd;
		}

        private void ImportPerLengthSequenceImpedances()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.PerLengthSequenceImpedance");
			if (cimObjects == null)
			{
				return;
			}

            foreach (KeyValuePair<string, object> cimObjectPair in cimObjects)
            {
                FTN.PerLengthSequenceImpedance cimObject = cimObjectPair.Value as FTN.PerLengthSequenceImpedance;

                ResourceDescription rd = CreatePerLengthSequenceImpedanceResourceDescription(cimObject);
				if (rd != null)
				{
					delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
					report.Report.Append("PerLengthSequenceImpedance ID = ").Append(cimObject.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
				}
				else
				{
					report.Report.Append("PerLengthSequenceImpedance ID = ").Append(cimObject.ID).AppendLine(" FAILED to be converted");
				}
			}
            report.Report.AppendLine();
        }

        private ResourceDescription CreatePerLengthSequenceImpedanceResourceDescription(FTN.PerLengthSequenceImpedance cimPerLengthSequenceImpedance)
        {
			ResourceDescription rd = null;
            if (cimPerLengthSequenceImpedance != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.PERLENGTHSEQUENCEIMPEDANCE, importHelper.CheckOutIndexForDMSType(DMSType.PERLENGTHSEQUENCEIMPEDANCE));
				rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimPerLengthSequenceImpedance.ID, gid);
                PowerTransformerConverter.PopulatePerLengthSequenceImpedanceProperties(cimPerLengthSequenceImpedance, rd);
            }
			return rd;
		}

        private void ImportSeriesCompensators()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.SeriesCompensator");
            if (cimObjects == null)
            {
                return;
            }

			foreach (KeyValuePair<string, object> cimObjectPair in cimObjects)
			{
				FTN.SeriesCompensator cimObject = cimObjectPair.Value as FTN.SeriesCompensator;

				ResourceDescription rd = CreateSeriesCompensatorResourceDescription(cimObject);
				if (rd != null)
				{
					delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
					report.Report.Append("SeriesCompensator ID = ").Append(cimObject.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
				}
				else
				{
					report.Report.Append("SeriesCompensator ID = ").Append(cimObject.ID).AppendLine(" FAILED to be converted");
				}
			}
			report.Report.AppendLine();
		}

        private ResourceDescription CreateSeriesCompensatorResourceDescription(FTN.SeriesCompensator cimSeriesCompensator)
        {
			ResourceDescription rd = null;
            if (cimSeriesCompensator != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.SERIESCOMPENSATOR, importHelper.CheckOutIndexForDMSType(DMSType.SERIESCOMPENSATOR));
				rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimSeriesCompensator.ID, gid);

                PowerTransformerConverter.PopulateSeriesCompensatorProperties(cimSeriesCompensator, rd);
            }
			return rd;
		}

        private void ImportDCLineSegments()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.DCLineSegment");
            if (cimObjects == null)
            {
                return;
            }

			foreach (KeyValuePair<string, object> cimObjectPair in cimObjects)
			{
				FTN.DCLineSegment cimObject = cimObjectPair.Value as FTN.DCLineSegment;

				ResourceDescription rd = CreateDCLineSegmentResourceDescription(cimObject);
				if (rd != null)
				{
					delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
					report.Report.Append("DCLineSegment ID = ").Append(cimObject.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
				}
				else
				{
					report.Report.Append("DCLineSegment ID = ").Append(cimObject.ID).AppendLine(" FAILED to be converted");
				}
			}
            report.Report.AppendLine();
        }

        private ResourceDescription CreateDCLineSegmentResourceDescription(FTN.DCLineSegment cimDCLineSegment)
        {
			ResourceDescription rd = null;
            if (cimDCLineSegment != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.DCLINESEGMENT, importHelper.CheckOutIndexForDMSType(DMSType.DCLINESEGMENT));
				rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimDCLineSegment.ID, gid);

                PowerTransformerConverter.PopulateDCLineSegmentProperties(cimDCLineSegment, rd);
            }
			return rd;
		}

        private void ImportACLineSegments()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.ACLineSegment");
            if (cimObjects == null)
            {
                return;
            }

            foreach (KeyValuePair<string, object> cimObjectPair in cimObjects)
            {
                FTN.ACLineSegment cimObject = cimObjectPair.Value as FTN.ACLineSegment;

                ResourceDescription rd = CreateACLineSegmentResourceDescription(cimObject);
                if (rd != null)
				{
                    delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                    report.Report.Append("ACLineSegment ID = ").Append(cimObject.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                }
                else
                {
                    report.Report.Append("ACLineSegment ID = ").Append(cimObject.ID).AppendLine(" FAILED to be converted");
                }
            }
			report.Report.AppendLine();
		}

        private ResourceDescription CreateACLineSegmentResourceDescription(FTN.ACLineSegment cimACLineSegment)
        {
            ResourceDescription rd = null;
            if (cimACLineSegment != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ACLINESEGMENT, importHelper.CheckOutIndexForDMSType(DMSType.ACLINESEGMENT));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimACLineSegment.ID, gid);

                PowerTransformerConverter.PopulateACLineSegmentProperties(cimACLineSegment, rd, importHelper, report);
            }

            return rd;
        }

        private void ImportTerminals()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.Terminal");
            if (cimObjects == null)
            {
                return;
            }

            foreach (KeyValuePair<string, object> cimObjectPair in cimObjects)
            {
                FTN.Terminal cimObject = cimObjectPair.Value as FTN.Terminal;

                ResourceDescription rd = CreateTerminalResourceDescription(cimObject);
                if (rd != null)
                {
                    delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                    report.Report.Append("Terminal ID = ").Append(cimObject.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                }
                else
                {
                    report.Report.Append("Terminal ID = ").Append(cimObject.ID).AppendLine(" FAILED to be converted");
                }
                report.Report.AppendLine();
            }

            report.Report.AppendLine();
        }

        private ResourceDescription CreateTerminalResourceDescription(FTN.Terminal cimTerminal)
        {
			ResourceDescription rd = null;
            if (cimTerminal != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.TERMINAL, importHelper.CheckOutIndexForDMSType(DMSType.TERMINAL)); rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimTerminal.ID, gid);

				PowerTransformerConverter.PopulateTerminalProperties(cimTerminal, rd, importHelper, report);			}
			return rd;
		}
		#endregion Import
	}
}

