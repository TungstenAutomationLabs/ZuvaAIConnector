/*
 * tungstenlabs.integration.zuvaai
 *
 * End User License Agreement (EULA)
 *
 * IMPORTANT: PLEASE READ THIS AGREEMENT CAREFULLY BEFORE USING THIS SOFTWARE.
 *
 * 1. GRANT OF LICENSE: Tungsten Automation grants you a limited, non-exclusive,
 * non-transferable, and revocable license to use this software solely for the
 * purposes described in the documentation accompanying the software.
 *
 * 2. RESTRICTIONS: You may not sublicense, rent, lease, sell, distribute,
 * redistribute, assign, or otherwise transfer your rights to this software.
 * You may not reverse engineer, decompile, or disassemble this software,
 * except and only to the extent that such activity is expressly permitted by
 * applicable law notwithstanding this limitation.
 *
 * 3. COPYRIGHT: This software is protected by copyright laws and international
 * copyright treaties, as well as other intellectual property laws and treaties.
 *
 * 4. DISCLAIMER OF WARRANTY: THIS SOFTWARE IS PROVIDED "AS IS" AND ANY EXPRESS
 * OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN
 * NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * 5. TERMINATION: Without prejudice to any other rights, Tungsten Automation may
 * terminate this EULA if you fail to comply with the terms and conditions of this
 * EULA. In such event, you must destroy all copies of the software and all of its
 * component parts.
 *
 * 6. GOVERNING LAW: This agreement shall be governed by the laws of USA,
 * without regard to conflicts of laws principles. Any disputes arising hereunder shall
 * be subject to the exclusive jurisdiction of the courts of USA.
 *
 * 7. ENTIRE AGREEMENT: This EULA constitutes the entire agreement between you and
 * Tungsten Automation relating to the software and supersedes all prior or contemporaneous
 * understandings regarding such subject matter. No amendment to or modification of this
 * EULA will be binding unless made in writing and signed by Tungsten Automation.
 *
 * Tungsten Automation
 * www.tungstenautomation.com
 * 09/30/2024
 */

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace tungstenlabs.integration.zuvaai
{
    public class APIHelper
    {
        private const string ZuvaUri = "https://us.app.zuva.ai/api/v2";

        #region "Public Methods"

        /// <summary>
        /// Uploads the base64-string based file to the Zuva DocAI service.
        /// </summary>
        /// <param name="base64Doc">Document as Base64 string</param>
        /// <param name="zuvaToken">Token provided by Zuva</param>
        /// <returns>Zuva File ID.</returns>
        public string ZuvaUploadFileBase64String(string base64Doc, string zuvaToken)
        {
            if (base64Doc.Trim().Length == 0)
            {
                throw new Exception("The base64Doc needs to be populated");
            }

            if (zuvaToken.Trim().Length == 0)
            {
                throw new Exception("The zuvaToken needs to be populated");
            }

            string text = "Nothing read";
            byte[] payload = Convert.FromBase64String(base64Doc);

            //Now let's send the file to Zuva
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ZuvaUri + "/files");
            httpWebRequest.Headers.Add("Authorization", FormatZuvaAuthToken(zuvaToken));
            httpWebRequest.ContentType = "application/pdf";
            httpWebRequest.ContentLength = payload.Length;
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";

            // Payload
            using (Stream sw = httpWebRequest.GetRequestStream())
            {
                sw.Write(payload, 0, payload.Length);
                sw.Flush();
            }

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            return GetJSONPropertyValue(text, "file_id");
        }

        /// <summary>
        /// Uploads the specified TotalAgility document - based on ID - to the Zuva DocAI Service
        /// </summary>
        /// <param name="docID">TotalAgility Document ID.</param>
        /// <param name="ktaSDKUrl">Complete URL for TotalAgility's SDK instance.</param>
        /// <param name="sessionID">TotalAgility Session ID.</param>
        /// <param name="zuvaToken">Token provided by Zuva; please include 'Bearer' before it.</param>
        /// <returns>Zuva File ID.</returns>
        public string ZuvaUploadFileKTASDK(string docID, string ktaSDKUrl, string sessionID, string zuvaToken)
        {
            if (docID.Trim().Length == 0)
            {
                throw new Exception("The TotalAgility docID needs to be populated");
            }

            if ((ktaSDKUrl.Trim().Length == 0) || (sessionID.Trim().Length == 0))
            {
                throw new Exception("The TotalAgility API information needs to be populated");
            }

            if (zuvaToken.Trim().Length == 0)
            {
                throw new Exception("The zuvaToken needs to be populated");
            }

            string text = "Nothing read";
            byte[] payload = GetKTADocumentFile(docID, ktaSDKUrl, sessionID);

            //Now let's send the file to Zuva
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ZuvaUri + "/files");
            httpWebRequest.Headers.Add("Authorization", FormatZuvaAuthToken(zuvaToken));
            httpWebRequest.ContentType = "application/pdf";
            httpWebRequest.ContentLength = payload.Length;
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";

            // Payload
            using (Stream sw = httpWebRequest.GetRequestStream())
            {
                sw.Write(payload, 0, payload.Length);
                sw.Flush();
            }

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            return GetJSONPropertyValue(text, "file_id");
        }

        /// <summary>
        /// Gets the classification result from Zuva; you first need to run ZuvaUploadFile to get the file ID
        /// </summary>
        /// <param name="zuvaFileID">File ID provided by Zuva.</param>
        /// <param name="zuvaToken">Token provided by Zuva.</param>
        /// <returns>Classification result from Zuva.</returns>
        public string ZuvaGetClassificationResult(string zuvaFileID, string zuvaToken)
        {
            if (zuvaFileID.Trim().Length == 0)
            {
                throw new Exception("zuvaFileID needs to be populated");
            }

            if (zuvaToken.Trim().Length == 0)
            {
                throw new Exception("zuvaToken needs to be populated");
            }

            string text = "Nothing read";
            string json = "{\"file_ids\": [ \"" + zuvaFileID + "\" ] }";

            // Convert the JSON data to bytes
            byte[] jsonDataBytes = Encoding.UTF8.GetBytes(json);

            //Send the classification request to Zuva
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ZuvaUri + "/classification");
            httpWebRequest.Headers.Add("Authorization", FormatZuvaAuthToken(zuvaToken));
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentLength = jsonDataBytes.Length;

            // Write the JSON data to the request stream
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(jsonDataBytes, 0, jsonDataBytes.Length);
                requestStream.Close();
            }

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            string requestid = GetJSONPropertyValue(text, "request_id");
            string status = "queued";

            // Poll Zuva until the classification is complete
            DateTime startTime = DateTime.Now;
            while ((status == "queued") || (status == "processing"))
            {
                Thread.Sleep(5000);
                httpWebRequest = (HttpWebRequest)WebRequest.Create(ZuvaUri + "/classification/" + requestid);
                httpWebRequest.Headers.Add("Authorization", FormatZuvaAuthToken(zuvaToken));
                httpWebRequest.Method = "GET";
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                }
                status = GetJSONPropertyValue(text, "status");

                // Optional: Break out if the process is taking too long
                if ((DateTime.Now - startTime).TotalMinutes > 10) // Timeout after 10 minutes
                {
                    throw new TimeoutException("Zuva processing timed out after 10 minutes.");
                }
            }

            if (status != "complete")
                throw new Exception("Zuva Classification threw an error: " + text);

            return GetJSONPropertyValue(text, "classification");
        }

        /// <summary>
        /// Gets the extraction results from Zuva for the file ID provided.
        /// Method synchronously polls for Zuva's process to complete.
        /// </summary>
        /// <param name="zuvaFileID">File ID provided by Zuva.</param>
        /// <param name="zuvaFieldList">Comma-delimited list of fields to be extracted.</param>
        /// <param name="zuvaToken">Token provided by Zuva.</param>
        /// <returns>JSON representation of the extraction results; this also included eOCR information.</returns>
        public string ZuvaGetExtractionResult(string zuvaFileID, string zuvaFieldList, string zuvaToken)
        {
            string result;

            if (zuvaFileID.Trim().Length == 0)
            {
                throw new Exception("zuvaFileID needs to be populated");
            }

            if (zuvaToken.Trim().Length == 0)
            {
                throw new Exception("zuvaToken needs to be populated");
            }

            string json = "{\"field_ids\": [ " + EscapeFieldList(zuvaFieldList) + " ], \"file_ids\": [ \"" + zuvaFileID + "\" ]}";

            // Convert the JSON data to bytes
            byte[] jsonDataBytes = Encoding.UTF8.GetBytes(json);

            //Send the classification request to Zuva
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ZuvaUri + "/extraction");
            httpWebRequest.Headers.Add("Authorization", FormatZuvaAuthToken(zuvaToken));
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentLength = jsonDataBytes.Length;

            // Write the JSON data to the request stream
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(jsonDataBytes, 0, jsonDataBytes.Length);
                requestStream.Close();
            }

            string text;
            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            string requestId = GetJSONPropertyValue(text, "request_id");
            string status = "queued";

            // Poll Zuva until the extraction is complete
            DateTime startTime = DateTime.Now;
            while (status == "queued" || status == "processing")
            {
                // Sleep for 5 seconds to avoid excessive polling
                Thread.Sleep(5000);

                httpWebRequest = (HttpWebRequest)WebRequest.Create(ZuvaUri + "/extraction/" + requestId);
                httpWebRequest.Headers.Add("Authorization", FormatZuvaAuthToken(zuvaToken));
                httpWebRequest.Method = "GET";

                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                }

                status = GetJSONPropertyValue(text, "status");

                // Optional: Break out if the process is taking too long
                if ((DateTime.Now - startTime).TotalMinutes > 10) // Timeout after 10 minutes
                {
                    throw new TimeoutException("Zuva processing timed out after 10 minutes.");
                }
            }

            if (status != "complete")
                throw new Exception("Zuva Extraction threw an error: " + text);

            // Retrieve the extraction results
            httpWebRequest = (HttpWebRequest)WebRequest.Create(ZuvaUri + "/extraction/" + requestId + "/results/text");
            httpWebRequest.Headers.Add("Authorization", FormatZuvaAuthToken(zuvaToken));
            httpWebRequest.Method = "GET";

            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// Gets the Zuva raw extraction results and turns it into a 3-dimension JSON array.
        /// </summary>
        /// <param name="extractionResult">Resulting JSON from calling the ZuvaGetExtractionResult method.</param>
        /// <returns>Json Array.</returns>
        public string GetJsonArrayFromExtraction(string extractionResult)
        {
            try
            {
                var resultList = new List<string[]>();

                JObject jsonObject = JObject.Parse(extractionResult);
                var resultsArray = jsonObject["results"].Children();

                foreach (var result in resultsArray)
                {
                    var resultFieldId = result["field_id"].ToString();
                    var extractionsArray = result["extractions"].Children();
                    foreach (var extraction in extractionsArray)
                    {
                        var text = extraction["text"].ToString();
                        resultList.Add(new string[] { resultFieldId, text, "" });
                    }
                }

                string[,] resultArray = new string[resultList.Count, 3];
                for (int i = 0; i < resultList.Count; i++)
                {
                    resultArray[i, 0] = resultList[i][0];
                    resultArray[i, 1] = resultList[i][1];
                    resultArray[i, 2] = resultList[i][2];
                }

                return JsonBuilder.BuildJson(new string[] { "Key", "Value1", "Value2" }, resultArray);
            }
            catch (System.NullReferenceException ex)
            {
                string[,] resultArray = new string[1, 3];
                resultArray[0, 0] = "Zuva Extraction";
                resultArray[0, 1] = "No Extraction Returned";
                resultArray[0, 2] = "";

                return JsonBuilder.BuildJson(new string[] { "Key", "Value1", "Value2" }, resultArray);
            }
        }

        public List<string> GetZuvaExtraction(List<ZuvaRequestInfo> list) 
        {
            List<string> result = new List<string>();
            string temp = "";
            foreach (ZuvaRequestInfo info in list)
            {
                foreach (ZuvaFieldConfig fieldconfig in info.ZuvaFieldConfigCollection)
                {
                    string extractionresult = ZuvaGetExtractionResult(info.ZuvaFileID, fieldconfig.ZuvaFieldIDs, info.ZuvaToken);
                    if (!extractionresult.ToLower().Contains("null"))
                    {
                        switch (fieldconfig.TransformationType)
                        {
                            case 0: break;
                            case 1:
                                temp = (GetFirstFromExtractionResultForField(info.ZuvaFileID, extractionresult)) + " -- ";
                                break;

                            case 2:
                                temp = (ConcatExtractionResultsForField(info.ZuvaFileID, extractionresult)) + " -- ";
                                break;

                            default:
                                temp = ("TransformationType Unknown -- ");
                                break;
                        }

                        if (fieldconfig.NormalizationType > 0 && fieldconfig.NormalizationType < 4)
                        {
                            string norm = "";
                            switch (fieldconfig.NormalizationType)
                            {
                                case 1: norm = "dates"; break;
                                case 2: norm = "currencies"; break;
                                case 3: norm = "durations"; break;
                            }
                            temp = temp + GetNormalizationFieldFromExtractionResultForField(extractionresult, info.ZuvaFileID, norm);
                        }

                        if (temp.EndsWith(" -- "))
                        {
                            temp = temp.Substring(0, temp.Length - " -- ".Length);
                        }

                        result.Add(temp);

                    }
                    else
                        result.Add("No extraction returned");
                }
            }

            return result;
        }

        /// <summary>
        /// Concatenates all text extracted from the results for the specified Field ID.
        /// </summary>
        /// <param name="zuvaFieldID">Field ID to filter results.</param>
        /// <param name="extractionResult">Resulting JSON from calling the ZuvaGetExtractionResult method.</param>
        /// <returns>Concatenated text results.</returns>
        public string GetAnswerResultsForField(string zuvaFieldID, string extractionResult)
        {
            try
            {
                var resultList = new List<string[]>();

                JObject jsonObject = JObject.Parse(extractionResult);
                var resultsArray = jsonObject["results"].Children();

                foreach (var result in resultsArray)
                {
                    var resultFieldId = result["field_id"].ToString();
                    if (FindValueInList(resultFieldId, zuvaFieldID))
                    {
                        var answersArray = result["answers"].Children();
                        string option = "Option: " + answersArray.First()["option"];
                        string answer = "Answer: " + answersArray.First()["value"];
                        resultList.Add(new string[] { "Answer", option, answer });

                        var extractionsArray = result["extractions"].Children();
                        foreach (var extraction in extractionsArray)
                        {
                            var spansArray = extraction["spans"].Children();
                            string pages = "Page(s): " + spansArray.First()["pages"]["start"] + " - " + spansArray.First()["pages"]["end"];
                            string text = "Text: " + extraction["text"];
                            resultList.Add(new string[] { "Supporting Paragraph", pages, text });
                        }
                    }
                }

                string[,] resultArray = new string[resultList.Count, 3];
                for (int i = 0; i < resultList.Count; i++)
                {
                    resultArray[i, 0] = resultList[i][0];
                    resultArray[i, 1] = resultList[i][1];
                    resultArray[i, 2] = resultList[i][2];
                }

                return JsonBuilder.BuildJson(new string[] { "Key", "Value1", "Value2" }, resultArray);
            }
            catch (System.NullReferenceException ex)
            {
                string[,] resultArray = new string[1, 3];
                resultArray[0, 0] = "Answer";
                resultArray[0, 1] = "No Answer Returned";
                resultArray[0, 2] = "";

                return JsonBuilder.BuildJson(new string[] { "Key", "Value1", "Value2" }, resultArray);
            }
        }

        // <summary>
        /// Gets the extraction results from Zuva for the file ID provided & concatenates it to the string provided.
        /// Method includes a 5 second wait in the extraction loop, while it waits for Zuva's process to complete.
        /// </summary>
        /// <param name="zuvaFileID">File ID provided by Zuva.</param>
        /// <param name="zuvaFieldList">Comma-delimited list of fields to be extracted.</param>
        /// <param name="zuvaToken">Token provided by Zuva; please include 'Bearer' before it.</param>
        /// <param name="stringToConcat">String to concatenate the results to, separated by "--".</param>
        /// <returns>Concatenated string with the extraction results.</returns>
        public string ZuvaExtractAndConcat(string zuvaFileID, string zuvaFieldList, string zuvaToken, string stringToConcat)
        {
            string result = "";

            if ((zuvaFileID != "") && (zuvaFieldList != ""))
            {
                string extraction = ZuvaGetExtractionResult(zuvaFileID, zuvaFieldList, zuvaToken);
                string extractionresults = ConcatExtractionResultsForField(zuvaFieldList, extraction);

                if (stringToConcat.Length > 0)
                    result = stringToConcat + " -- " + extractionresults;
                else
                    result = extractionresults;
            }

            return result;
        }



        #endregion "Public Methods"

        #region "Private Methods"

        /// <summary>
        /// Concatenates all text extracted from the results for the specified Field ID.
        /// </summary>
        /// <param name="zuvaFieldID">Field ID to filter results.</param>
        /// <param name="extractionResult">Resulting JSON from calling the ZuvaGetExtractionResult method.</param>
        /// <returns>Concatenated text results.</returns>
        private string ConcatExtractionResultsForField(string zuvaFieldID, string extractionResult)
        {
            string concatenatedText = "";

            JObject jsonObject = JObject.Parse(extractionResult);
            var resultsArray = jsonObject["results"].Children();

            foreach (var result in resultsArray)
            {
                var resultFieldId = result["field_id"].ToString();
                if (FindValueInList(resultFieldId, zuvaFieldID))
                {
                    var extractionsArray = result["extractions"].Children();
                    foreach (var extraction in extractionsArray)
                    {
                        //var text = CleanHtml(extraction["text"].ToString());
                        var text = extraction["text"].ToString();
                        if (concatenatedText.Length == 0)
                            concatenatedText = CleanHtml(text);
                        else
                            concatenatedText += " || " + CleanHtml(text);
                    }
                }
            }
            return concatenatedText;
        }

        /// <summary>
        /// Gets the Zuva-normalized information for the given extraction result
        /// </summary>
        /// <param name="zuvaFieldID">Field ID to filter results.</param>
        /// <param name="extractionResult">Resulting JSON from calling the ZuvaGetExtractionResult method.</param>
        /// <param name="normalizationField">String which specifies which normalization entry to get; options are "currencies", "dates", "durations".</param>
        /// <returns>The sum of the two integers.</returns>
        private string GetNormalizationFieldFromExtractionResultForField(string jsonString, string zuvaFieldID, string normalizationField)
        {
            JObject jsonObject = JObject.Parse(jsonString);
            var resultsArray = jsonObject["results"].Children();

            foreach (var result in resultsArray)
            {
                var resultFieldId = result["field_id"].ToString();
                if (FindValueInList(resultFieldId, zuvaFieldID))
                {
                    var extractionsArray = result["extractions"].Children();
                    foreach (var extraction in extractionsArray)
                    {

                        //if (!string.IsNullOrEmpty(normalizationElement.First.ToString()))
                        if (extraction.First.ToString().Contains(normalizationField))
                        {
                            var normalizationElement = extraction[normalizationField];

                            switch (normalizationField)
                            {
                                case "currencies":
                                    string firstCurrency = normalizationElement.First.ToString(); //GetJSONPropertyValue(extraction, "0");

                                    if (!string.IsNullOrEmpty(firstCurrency))
                                    {
                                        string value = GetJSONPropertyValue(firstCurrency, "value");
                                        string symbol = GetJSONPropertyValue(firstCurrency, "symbol");
                                        string precision = GetJSONPropertyValue(firstCurrency, "precision");

                                        if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(symbol) && !string.IsNullOrEmpty(precision))
                                        {
                                            decimal decimalValue;
                                            if (decimal.TryParse(value, out decimalValue))
                                            {
                                                int intPrecision;
                                                if (int.TryParse(precision, out intPrecision))
                                                {
                                                    return $"{symbol}{(decimalValue / (decimal)Math.Pow(10, intPrecision)):F2}";
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case "dates":
                                    string firstDate = normalizationElement.First.ToString(); //GetJSONPropertyValue(extraction, "0");

                                    if (!string.IsNullOrEmpty(firstDate))
                                    {
                                        string day = GetJSONPropertyValue(firstDate, "day");
                                        string month = GetJSONPropertyValue(firstDate, "month");
                                        string year = GetJSONPropertyValue(firstDate, "year");

                                        if (!string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
                                        {
                                            return $"{month:D2}/{day:D2}/{year:D4}";
                                        }
                                    }
                                    break;

                                case "durations":
                                    string firstDuration = normalizationElement.First.ToString(); //GetJSONPropertyValue(normalizationElement, "0");

                                    if (!string.IsNullOrEmpty(firstDuration))
                                    {
                                        string unit = GetJSONPropertyValue(firstDuration, "unit");
                                        string value = GetJSONPropertyValue(firstDuration, "value");

                                        if (!string.IsNullOrEmpty(unit) && !string.IsNullOrEmpty(value))
                                        {
                                            return $"{value} {unit}";
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            return null; // Return null if the normalization field is not found or the properties are missing
        }

        private byte[] GetKTADocumentFile(string docID, string ktaSDKUrl, string sessionID)
        {
            byte[] result = new byte[1];
            byte[] buffer = new byte[4096];
            //string fileType = "pdf";
            string status = "OK";

            try
            {
                //Setting the URi and calling the get document API
                var KTAGetDocumentFile = ktaSDKUrl + "/CaptureDocumentService.svc/json/GetDocumentFile2";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(KTAGetDocumentFile);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                // CONSTRUCT JSON Payload
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"sessionId\":\"" + sessionID + "\",\"reportingData\": {\"Station\": \"\", \"MarkCompleted\": false }, \"documentId\":\"" + docID + "\", \"documentFileOptions\": { \"FileType\": \"\", \"IncludeAnnotations\": 0 } }";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream receiveStream = httpWebResponse.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader readStream = new StreamReader(receiveStream, encode);
                int streamContentLength = unchecked((int)httpWebResponse.ContentLength);

                using (Stream responseStream = httpWebResponse.GetResponseStream())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = responseStream.Read(buffer, 0, buffer.Length);
                            memoryStream.Write(buffer, 0, count);
                        } while (count != 0);

                        result = memoryStream.ToArray();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                status = "An error occured: " + ex.ToString();
                return result;
            }
        }

        /// <summary>
        /// Gets the first instance of extraction results for the specified Field ID.
        /// </summary>
        /// <param name="zuvaFieldID">Field ID to filter results.</param>
        /// <param name="extractionResult">Resulting JSON from calling the ZuvaGetExtractionResult method.</param>
        /// <returns>First instance of text results.</returns>
        private string GetFirstFromExtractionResultForField(string zuvaFieldID, string extractionResult)
        {
            JObject jsonObject = JObject.Parse(extractionResult);
            var resultsArray = jsonObject["results"].Children();

            foreach (var result in resultsArray)
            {
                var resultFieldId = result["field_id"].ToString();
                if (FindValueInList(resultFieldId, zuvaFieldID))
                {
                    var extractionsArray = result["extractions"].Children();
                    foreach (var extraction in extractionsArray)
                    {
                        var text = extraction["text"]?.ToString();
                        if (text != null)
                        {
                            return text;
                        }
                    }
                }
            }

            return null; // Return null if no matching "text" property is found for the specified "field_id"
        }
        private static string GetJSONPropertyValue(string jsonString, string propertyName)
        {
            JToken root = JToken.Parse(jsonString);

            if (root.Type == JTokenType.Array)
            {
                foreach (JToken item in root.Children())
                {
                    string result = GetJSONPropertyValue(item.ToString(), propertyName);
                    if (!string.IsNullOrEmpty(result))
                        return result;
                }
            }
            else if (root.Type == JTokenType.Object)
            {
                JToken propertyValue = root[propertyName];
                if (propertyValue != null)
                    return propertyValue.ToString();

                foreach (JProperty property in root.Children<JProperty>())
                {
                    string result = GetJSONPropertyValue(property.Value.ToString(), propertyName);
                    if (!string.IsNullOrEmpty(result))
                        return result;
                }
            }

            return string.Empty; // Property not found, return an empty string or throw an exception as needed
        }

        private static string EscapeFieldList(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Check if the string is already formatted
            if (input.StartsWith("\"") && input.EndsWith("\""))
                return input;

            // Split the string by commas
            string[] parts = input.Split(',');
            string[] modifiedParts = parts.Select(part => part.Replace("\"", "").Trim()).ToArray();

            // Escape each part and join them with commas
            string formattedString = string.Join(",", modifiedParts.Select(part => $"\"{part.Trim()}\""));

            return formattedString;
        }

        private string FormatZuvaAuthToken(string authToken)
        {
            if (authToken.Contains("Bearer"))
                return authToken;
            else
                return "Bearer " + authToken;
        }

        private string CleanHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            // Remove HTML tags using regular expressions
            string cleanText = Regex.Replace(html, "<[^>]*>", string.Empty);

            // Manually decode common HTML entities
            cleanText = DecodeHtmlEntities(cleanText);

            return cleanText;
        }

        private string DecodeHtmlEntities(string text)
        {
            // Define a dictionary of common HTML entities and their replacements
            var htmlEntities = new Dictionary<string, string>
            {
                { "&amp;", "&" },
                { "&lt;", "<" },
                { "&gt;", ">" },
                { "&quot;", "\"" },
                { "&apos;", "'" }
                // Add more entities as needed
            };

            // Replace HTML entities in the text
            foreach (var entity in htmlEntities)
            {
                text = text.Replace(entity.Key, entity.Value);
            }

            return text;
        }

        private bool FindValueInList(string valueToFind, string commaDelimitedString)
        {
            // Split the comma-delimited string into an array and clean up extra double-quotes.
            string[] values = commaDelimitedString.Split(',')
                .Select(s => s.Trim().Trim('"'))
                .ToArray();

            // Check if the value to find exists in the cleaned array.
            return Array.Exists(values, val => val.Equals(valueToFind, StringComparison.OrdinalIgnoreCase));
        }

        public static string GetMimeType(string fileExtension, byte[] fileBytes)
        {
            // Attempt to get MIME type based on the file extension
            if (!fileExtension.StartsWith(".")) { fileExtension = "." + fileExtension; }
            string mimeType = "application/octet-stream"; // Default unknown type
            if (!string.IsNullOrWhiteSpace(fileExtension))
            {
                try
                {
                    mimeType = System.Web.MimeMapping.GetMimeMapping(fileExtension);
                }
                catch
                {
                    // Handle exceptions if the MimeMapping fails or is unavailable
                }

                // If a valid MIME type was retrieved from the extension, return it
                if (!string.IsNullOrWhiteSpace(mimeType) && !mimeType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
                {
                    return mimeType;
                }
            }

            // If extension-based lookup failed, use byte-signature-based lookup
            if (fileBytes == null || fileBytes.Length < 4)
                return mimeType;  // Default unknown type

            // Define file signatures in byte arrays
            byte[] jpg = new byte[] { 0xFF, 0xD8 };
            byte[] png = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
            byte[] gif = new byte[] { 0x47, 0x49, 0x46 };
            byte[] tiffI = new byte[] { 0x49, 0x49, 0x2A, 0x00 };
            byte[] tiffM = new byte[] { 0x4D, 0x4D, 0x00, 0x2A };
            byte[] pdf = new byte[] { 0x25, 0x50, 0x44, 0x46 };

            // Compare file signature with defined signatures
            if (fileBytes.Take(jpg.Length).SequenceEqual(jpg))
                return "image/jpeg";

            if (fileBytes.Take(png.Length).SequenceEqual(png))
                return "image/png";

            if (fileBytes.Take(gif.Length).SequenceEqual(gif))
                return "image/gif";

            if (fileBytes.Take(tiffI.Length).SequenceEqual(tiffI) || fileBytes.Take(tiffM.Length).SequenceEqual(tiffM))
                return "image/tiff";

            if (fileBytes.Take(pdf.Length).SequenceEqual(pdf))
                return "application/pdf";

            return mimeType;  // Return default type if none of the byte signatures match
        }

        #endregion "Private Methods"
    }
}