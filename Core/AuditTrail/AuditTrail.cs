//-----------------------------------------------------------------------
// <copyright file="AuditTrail.cs" company="Lifeprojects.de">
//     Class: AuditTrail
//     Copyright © Lifeprojects.de 2021
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>22.01.2021</date>
//
// <summary>
// Die Klasse stellt Methoden zum erstellen eines AuditTrail Eintrages zur Verfügung
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.AuditTrail
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using ModernBaseLibrary.Comparer;
    using ModernBaseLibrary.Core;

    using PasswortNET.Model;

    public static class AuditTrail
    {
        private static readonly List<string> IgnorProperties = null;

        static AuditTrail()
        {
            using(IgnorAuditTrailWords words = new IgnorAuditTrailWords())
            {
                IgnorProperties = words.IgnorProperties;
            }
        }

        public static OperationResult<AuditTrailResult> Create<TObject>(TObject firstObj, TObject secondObj)
        {
            bool operationResultState = false;
            AuditTrailResult result = null;

            if (firstObj != null && secondObj == null)
            {
                ChangeTracking trackingObject = CreateTrackingObjectForNew<TObject>(firstObj);

                result = new AuditTrailResult();
                result.Activity = ActionAuditTrail.New;
                result.TrackChanges = 1;
                result.TrackingObject = trackingObject;
                operationResultState = true;
            }
            else if (firstObj != null && secondObj != null)
            {
                string[] ignorProperty = IgnorProperties.ToArray();
                List<CompareResult> compareResult = CompareObject.GetDifferences(firstObj, secondObj, ignorProperty);

                if (compareResult != null && compareResult.Count > 0)
                {
                    ChangeTracking trackingObject = CreateTrackingObject(compareResult, secondObj);
                    if (trackingObject != null)
                    {
                        result = new AuditTrailResult();
                        result.Activity = ActionAuditTrail.Change;
                        result.TrackChanges = compareResult.Count(c => TestOfIgnorWords(c.PropertyName) == false);
                        result.TrackingObject = trackingObject;
                        operationResultState = true;
                    }
                    else
                    {
                        operationResultState = false;
                    }
                }
                else
                {
                    operationResultState = false;
                }
            }
            else if (firstObj == null && secondObj != null)
            {
                ChangeTracking trackingObject = CreateTrackingObjectForDelete<TObject>(secondObj);

                result = new AuditTrailResult();
                result.Activity = ActionAuditTrail.Delete;
                result.TrackChanges = 1;
                result.TrackingObject = trackingObject;
                operationResultState = true;
            }

            return OperationResult<AuditTrailResult>.SuccessResult(result, operationResultState);
        }

        private static ChangeTracking CreateTrackingObject<TObject>(List<CompareResult> compareResult, TObject obj)
        {
            ChangeTracking trackingObj = null;
            if (obj != null)
            {
                string id = GetObjectId(obj);
                string valueContentOld = GetValueFromCompareOld(compareResult);
                string valueContentNew = GetValueFromCompareNew(compareResult);
                if (valueContentOld != valueContentNew)
                {
                    trackingObj = new ChangeTracking();
                    trackingObj.Id = Guid.NewGuid();
                    trackingObj.ActivityTyp = ActionAuditTrail.Change.ToString();
                    trackingObj.ObjectId = id;
                    trackingObj.ObjectName = obj.GetType().Name;
                    trackingObj.OldValue = valueContentOld;
                    trackingObj.NewValue = valueContentNew;
                    trackingObj.CreatedBy = UserInfo.TS().CurrentDomainUser;
                    trackingObj.CreatedOn = UserInfo.TS().CurrentTime;
                    trackingObj.ModifiedBy = UserInfo.TS().CurrentDomainUser;
                    trackingObj.ModifiedOn = UserInfo.TS().CurrentTime;
                }
            }

            return trackingObj;
        }

        private static ChangeTracking CreateTrackingObjectForNew<TObject>(TObject obj)
        {
            ChangeTracking trackingObj = null;
            if (obj != null)
            {
                string id = GetObjectId(obj);
                string valueContent = GetValue(obj);
                trackingObj = new ChangeTracking();
                trackingObj.Id = Guid.NewGuid();
                trackingObj.ActivityTyp = ActionAuditTrail.New.ToString();
                trackingObj.ObjectId = id;
                trackingObj.ObjectName = obj.GetType().Name;
                trackingObj.NewValue = valueContent;
                trackingObj.OldValue = string.Empty;
                trackingObj.CreatedBy = UserInfo.TS().CurrentDomainUser;
                trackingObj.CreatedOn = UserInfo.TS().CurrentTime;
                trackingObj.ModifiedBy = UserInfo.TS().CurrentDomainUser;
                trackingObj.ModifiedOn = UserInfo.TS().CurrentTime;
            }

            return trackingObj;
        }

        private static ChangeTracking CreateTrackingObjectForDelete<TObject>(TObject obj)
        {
            ChangeTracking trackingObj = null;
            if (obj != null)
            {
                string id = GetObjectId(obj);
                string valueContent = GetValue(obj);
                trackingObj = new ChangeTracking();
                trackingObj.Id = Guid.NewGuid();
                trackingObj.ActivityTyp = ActionAuditTrail.Delete.ToString();
                trackingObj.ObjectId = id;
                trackingObj.ObjectName = obj.GetType().Name;
                trackingObj.NewValue = string.Empty;
                trackingObj.OldValue = valueContent;
                trackingObj.CreatedBy = UserInfo.TS().CurrentDomainUser;
                trackingObj.CreatedOn = UserInfo.TS().CurrentTime;
                trackingObj.ModifiedBy = UserInfo.TS().CurrentDomainUser;
                trackingObj.ModifiedOn = UserInfo.TS().CurrentTime;
            }

            return trackingObj;
        }

        private static string GetObjectId<TObject>(TObject obj)
        {
            string result = string.Empty;

            if (obj != null && obj is PasswordPin passwordPin)
            {
                result = passwordPin.Id.ToString();
            }

            return result;
        }

        private static string GetValue<TObject>(TObject obj)
        {
            string result = string.Empty;

            PropertyInfo[] propInfo = obj.GetType().GetProperties();
            StringBuilder outText = new StringBuilder();
            foreach (PropertyInfo propItem in propInfo)
            {
                if (TestOfIgnorWords(propItem.Name) == false)
                {
                    outText.AppendFormat($"{propItem.Name}: {propItem.GetValue(obj, null)}~");
                }
            }

            result = outText.ToString();

            return result;
        }

        private static string GetValueFromCompareOld(List<CompareResult> compareResult)
        {
            string result = string.Empty;
            StringBuilder outText = new StringBuilder();
            foreach (CompareResult item in compareResult)
            {
                if (TestOfIgnorWords(item.PropertyName) == false)
                {
                    outText.AppendFormat($"{item.PropertyName}: {item.FirstValue}~");
                }
            }

            result = outText.ToString();

            return result;
        }

        private static string GetValueFromCompareNew(List<CompareResult> compareResult)
        {
            string result = string.Empty;
            StringBuilder outText = new StringBuilder();
            foreach (CompareResult item in compareResult)
            {
                if (TestOfIgnorWords(item.PropertyName) == false)
                {
                    outText.AppendFormat($"{item.PropertyName}: {item.SecondValue}\n");
                }
            }

            result = outText.ToString();

            return result;
        }

        private static bool TestOfIgnorWords(string value)
        {
            return IgnorProperties.Any(p => p.ToLower() == value.ToLower());
        }
    }
}
