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
            
            string extraction = oAPIH.ZuvaGetExtractionResult(fileid, "98086156-f230-423c-b214-27f542e72708", zuvaToken);
            string json = oAPIH.GetJsonArrayFromExtraction(extraction);

            string answer = oAPIH.GetAnswerResultsForField("6ed92ac5-b394-45ee-962e-1ceb9750d95d", extraction);
            Assert.IsNotNull(answer);

            string concat = oAPIH.ConcatExtractionResultsForField("6ef55bcb-8814-4928-a9e0-e6ca7f27a73a", extraction);
            Assert.IsNotNull(concat);

            string first = oAPIH.GetFirstFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(first);

            string last = oAPIH.GetLastFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(last);

            string firstnum = oAPIH.GetFirstNumberFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(firstnum);

            string lastnum = oAPIH.GetLastNumberFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(lastnum);

            string sum = oAPIH.GetSumOfAllNumbersFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(sum);

            string firstdate = oAPIH.GetFirstDateFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(firstdate);

            string lastdate = oAPIH.GetLastDateFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(lastdate);

            string earliest = oAPIH.GetEarliestDateFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(earliest);

            string latest = oAPIH.GetLatestDateFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(latest);
        }
    }
}