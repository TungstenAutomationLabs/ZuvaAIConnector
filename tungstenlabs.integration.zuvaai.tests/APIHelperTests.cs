using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace tungstenlabs.integration.zuvaai.tests
{
    [TestClass]
    public class APIHelperTests
    {
        [TestMethod]
        public void ZuvaUploadFileKTASDK()
        {
            string documentID = @"d2f593f4-f047-42f8-b884-b202015ae18f";

            string zuvaToken = @"Bearer " + Constants.ZUVA_TOKEN;
            APIHelper oAPIH = new APIHelper();

            string fileid = oAPIH.ZuvaUploadFileKTASDK(documentID, Constants.TOTALAGILITY_API_URL, Constants.TOTALAGILITY_SESSION_ID, zuvaToken);
            Assert.IsNotNull(fileid);

            List<ZuvaFieldConfig> fields = new List<ZuvaFieldConfig>();

            ZuvaFieldConfig field = new ZuvaFieldConfig();
            field.ZuvaFieldIDs = "fc5ba010-671b-427f-82cb-95c02d4c704c";
            field.TransformationType = 1;
            field.NormalizationType = 1;
            fields.Add(field);

            field = new ZuvaFieldConfig();
            field.ZuvaFieldIDs = "25d677a1-70d0-43c2-9b36-d079733dd020";
            field.TransformationType = 1;
            field.NormalizationType = 0;
            fields.Add(field);

            field = new ZuvaFieldConfig();
            field.ZuvaFieldIDs = "98086156-f230-423c-b214-27f542e72708";
            field.TransformationType = 2;
            field.NormalizationType = 0;
            fields.Add(field);

            ZuvaRequestInfo zuvaRequestInfo = new ZuvaRequestInfo();
            zuvaRequestInfo.ZuvaFileID = fileid;
            zuvaRequestInfo.ZuvaToken = zuvaToken;
            zuvaRequestInfo.ZuvaFieldConfigCollection = fields;

            List<ZuvaRequestInfo> reqs = new List<ZuvaRequestInfo>();
            reqs.Add(zuvaRequestInfo);

            List<string> result = oAPIH.GetZuvaExtraction(reqs);
        }

        [TestMethod]
        public void ZuvaUploadAndExtractiontest()
        {
            string documentID = @"e3d77b59-16f5-4c6a-aaa6-b1f70172cd9b";

            string zuvaToken = @"Bearer " + Constants.ZUVA_TOKEN;
            APIHelper oAPIH = new APIHelper();

            string fileid = oAPIH.ZuvaUploadFileKTASDK(documentID, Constants.TOTALAGILITY_API_URL, Constants.TOTALAGILITY_SESSION_ID, zuvaToken);
        }

        [TestMethod]
        public void ZuvaGetClassificationResult()
        {
            string documentID = @"e3d77b59-16f5-4c6a-aaa6-b1f70172cd9b";

            string zuvaToken = @"Bearer " + Constants.ZUVA_TOKEN;
            APIHelper oAPIH = new APIHelper();

            //string fileid = @"test with specific file id";
            string fileid = oAPIH.ZuvaUploadFileKTASDK(documentID, Constants.TOTALAGILITY_API_URL, Constants.TOTALAGILITY_SESSION_ID, zuvaToken);
            
            string classification = oAPIH.ZuvaGetClassificationResult(fileid, zuvaToken);
        }

        [TestMethod]
        public void ZuvaGetExtractionResult()
        {
            string documentID = @"e3d77b59-16f5-4c6a-aaa6-b1f70172cd9b";

            string zuvaToken = @"Bearer " + Constants.ZUVA_TOKEN;
            APIHelper oAPIH = new APIHelper();

            //string fileid = @"test with specific file id";
            string fileid = oAPIH.ZuvaUploadFileKTASDK(documentID, Constants.TOTALAGILITY_API_URL, Constants.TOTALAGILITY_SESSION_ID, zuvaToken);
            
            string extraction = oAPIH.ZuvaGetExtractionResult(fileid, "98086156-f230-423c-b214-27f542e72708", zuvaToken);
        }

        [TestMethod]
        public void GetJsonArrayFromExtraction()
        {
            string documentID = @"e3d77b59-16f5-4c6a-aaa6-b1f70172cd9b";

            string zuvaToken = @"Bearer " + Constants.ZUVA_TOKEN;
            APIHelper oAPIH = new APIHelper();

            //string fileid = @"test with specific file id";
            string fileid = oAPIH.ZuvaUploadFileKTASDK(documentID, Constants.TOTALAGILITY_API_URL, Constants.TOTALAGILITY_SESSION_ID, zuvaToken);

            string extraction = oAPIH.ZuvaGetExtractionResult(fileid, "98086156-f230-423c-b214-27f542e72708", zuvaToken);
            string formattedResult = oAPIH.GetJsonArrayFromExtraction(extraction);
        }

        [TestMethod]
        public void GetAnswerResultsForField()
        {
            string documentID = @"e3d77b59-16f5-4c6a-aaa6-b1f70172cd9b";

            string zuvaToken = @"Bearer " + Constants.ZUVA_TOKEN;
            APIHelper oAPIH = new APIHelper();

            //string fileid = @"test with specific file id";
            string fileid = oAPIH.ZuvaUploadFileKTASDK(documentID, Constants.TOTALAGILITY_API_URL, Constants.TOTALAGILITY_SESSION_ID, zuvaToken);
            
            string extraction = oAPIH.ZuvaGetExtractionResult(fileid, "98086156-f230-423c-b214-27f542e72708", zuvaToken);
            string answer = oAPIH.GetAnswerResultsForField("6ed92ac5-b394-45ee-962e-1ceb9750d95d", extraction);

        }
    }
}