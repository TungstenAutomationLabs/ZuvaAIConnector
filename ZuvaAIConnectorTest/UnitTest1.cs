using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace tungstenlabs.integration.zuvaai
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestZuvaFlow()
        {
            //string zuvatoken = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJvcmdfaWQiOiJvcmdfcnhRQnFpQzlPeWdNM3VCOCIsInRvayI6ImNpMHJsN25lcW92Y3BlZDJkcmRnIiwidSI6ImF1dGgwfDYzZDMzYzBiMTNlZmRhMDY0ZDNiZDc0ZiJ9.Ox4Ijl_Xs0ifoxZSZR3kTspxGK9EphlaYEgX0ljbhOg";
            string zuvatoken = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJvcmdfaWQiOiJvcmdfQ3FZeHlNRGR1UVdmc0NOaiIsInRvayI6ImNuY2hhYWxzc2NmYzczZDE5ZzEwIiwidSI6ImF1dGgwfDY1ZDkwNTY3ODkyNzRmOGQ2YTIwNTQ1YSJ9.0DmDFhT1fvWXxZtYNeD__LSaYE2NzXtOCMkZWCEmA5Y";
            ZuvaAIConnector zuvaConnector = new ZuvaAIConnector();

            //string fileid = zuvaConnector.ZuvaUploadFileKTASDK("41422ecd-b26a-4a5e-ae2c-b12601119176", "https://ktacloudeco.ktaprt.kofaxcloud.com//services/sdk/", "2BDC955ED880C84B9CB52287D59EBF37", zuvatoken);
            string fileid = zuvaConnector.ZuvaUploadFileKTASDK("e3d77b59-16f5-4c6a-aaa6-b1f70172cd9b", "https://ktacloudeco-dev.ktaprt.dev.kofaxcloud.com/services/sdk/", "D2A967C768C7854B91C210DF77F118A4", zuvatoken);

            //Assert.IsNotNull(fileid);

            //string extraction = "{\"request_id\":\"crqlobiq936c73dq99h0\",\"file_id\":\"crqkiciq936c73crcfmg\",\"results\":[{\"field_id\":\"6ed92ac5-b394-45ee-962e-1ceb9750d95d\",\"field_name\":\"Does this contract renew automatically?\",\"extractions\":null}]}";

            //string classification = zuvaConnector.ZuvaGetClassificationResult(fileid, zuvatoken);
            //Assert.IsNotNull(classification);

            //string extraction = zuvaConnector.ZuvaGetExtractionResult("ck4r8pj95kjs73chq1c0", "668ee3b5-e15a-439f-9475-05a21755a5c1\", \"25d677a1-70d0-43c2-9b36-d079733dd020,\"c83868ae-269a-4a1b-b2af-c53e5f91efca\",\"e211dec8-5c81-41e6-9ec1-ef21afde98a5\",\"7394e722-9668-4f84-a846-c342fa15ad75\",98086156-f230-423c-b214-27f542e72708\", \"f743f363-1d8b-435b-8812-204a6d883834\"", zuvatoken);
            string extraction = zuvaConnector.ZuvaGetExtractionResult(fileid, "98086156-f230-423c-b214-27f542e72708", zuvatoken);
            //string extraction = zuvaConnector.ZuvaExtractAndConcat("ckmofsuu7qmc738nngig", "6ef55bcb-8814-4928-a9e0-e6ca7f27a73a", zuvatoken, "");
            //string extraction = zuvaConnector.ZuvaGetAnswerResult(fileid, "f743f363-1d8b-435b-8812-204a6d883834", zuvatoken);
            //Assert.IsNotNull(extraction);

            //string norm = zuvaConnector.GetNormalizationFieldFromExtractionResultForField(extraction, "e211dec8-5c81-41e6-9ec1-ef21afde98a5", "durations");
            //Assert.IsNotNull(norm);

            string json = zuvaConnector.GetJsonArrayFromExtraction(extraction);
            
            string answer = zuvaConnector.GetAnswerResultsForField("6ed92ac5-b394-45ee-962e-1ceb9750d95d", extraction);
            //Assert.IsNotNull(answer);

            string concat = zuvaConnector.ConcatExtractionResultsForField("6ef55bcb-8814-4928-a9e0-e6ca7f27a73a", extraction);
            Assert.IsNotNull(concat);

            string first = zuvaConnector.GetFirstFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(first);

            string last = zuvaConnector.GetLastFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(last);

            string firstnum = zuvaConnector.GetFirstNumberFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(firstnum);

            string lastnum = zuvaConnector.GetLastNumberFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(lastnum);

            string sum = zuvaConnector.GetSumOfAllNumbersFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(sum);

            string firstdate = zuvaConnector.GetFirstDateFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(firstdate);

            string lastdate = zuvaConnector.GetLastDateFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(lastdate);

            string earliest = zuvaConnector.GetEarliestDateFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(earliest);

            string latest = zuvaConnector.GetLatestDateFromExtractionResultForField("668ee3b5-e15a-439f-9475-05a21755a5c1", extraction);
            Assert.IsNotNull(latest);

        }
    }
}
