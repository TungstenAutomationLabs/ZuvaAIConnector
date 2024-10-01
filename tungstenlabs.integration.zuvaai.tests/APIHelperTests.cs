using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace tungstenlabs.integration.zuvaai.tests
{
    [TestClass]
    public class APIHelperTests
    {
        [TestMethod]
        public void ZuvaUploadFileKTASDK()
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