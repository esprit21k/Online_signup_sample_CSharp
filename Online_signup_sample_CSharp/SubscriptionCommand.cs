/* This file is part of Sign-up Page Sample.

The Sign-up Page Sample is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Sign-up Page Sample is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with The Sign-up Page Sample.  If not, see <http://www.gnu.org/licenses/>.

 * Program Work Flow
 * 1. Check if the subscription exists.
 * 2a. If it does exist, retrieve subscription_id.
 * 	2b. Edit the subscription and add to the new list.
 * 3a. If it does NOT exist.
 * 	3b. Add new subscription to list.
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Online_signup_sample_CSharp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Online_signup_sample_CSharp
{
    public class SubscriptionCommand
    {
        // Account information and validation.
        // Replace myName with the proper username and myKey with the API key
        // Get your free Trumpia API Key at http://api.trumpia.com
        // Replace with the list_name the subscription will be added to
        private string apiKey = "myKey";
        private string userName = "myName";
        private string listName = "MyContacts";

        // Class for REST request
        private RequestRest requestRest;

        private string subscriptionID;
        private string baseUrl = "http://api.trumpia.com/rest/v1/";

        public string Execute(string firstName, string lastName, string mobileNum)
        {
            JObject response;
            requestRest = new RequestRest();

            // Mobile number search.
            // If the subscription exists, it will grab the existing subscription_id and edit the subscription with (POST).
            // If the subscription does not exist, it will add it as a new record.
            if (SearchExist(firstName, lastName, mobileNum))
                response = PostSubscription(firstName, lastName, mobileNum);
            else
                response = PutSubscription(firstName, lastName, mobileNum);

            // Get report from response.
            return GetReport(response);
        }

        // This function will search if the mobile number exists in the database.
        private bool SearchExist(string firstName, string lastName, string mobileNum)
        {
            string searchType = "2";
            string requestUrl = baseUrl + userName + "/subscription/search?" +
                    "search_type=" + searchType +
                    "&search_data=" + mobileNum;
            requestRest.apiKey = apiKey;
            requestRest.requestUrl = requestUrl;

            JObject response = JObject.Parse(requestRest.Get());
            if (response["subscription_id_list"] != null)
            {
                subscriptionID = response.SelectToken("subscription_id_list[0]").ToString();
                return true;
            }
            return false;

        }

        // This function will edit a subscription.
        private JObject PostSubscription(string firstName, string lastName, string mobileNum)
        {
            string requestUrl = baseUrl + userName + "/subscription/" + subscriptionID;
            JObject jsonBody = Body(firstName, lastName, mobileNum);

            jsonBody.SelectToken("subscriptions[0].voice_device").Parent.Remove();
            jsonBody.SelectToken("subscriptions[0].mobile.number").Parent.Remove();

            string requestBody = jsonBody.ToString();
            requestRest.requestUrl = requestUrl;
            requestRest.requestBody = requestBody;
            JObject response = JObject.Parse(requestRest.Post());

            return response;
        }

        // This function will add a new subscription.
        private JObject PutSubscription(string firstName, string lastName, string mobileNum)
        {
            string requestUrl = baseUrl + userName + "/subscription";
            JObject jsonBody = Body(firstName, lastName, mobileNum);

            string requestBody = jsonBody.ToString();
            requestRest.requestUrl = requestUrl;
            requestRest.requestBody = requestBody;
            JObject response = JObject.Parse(requestRest.Put());

            return response;
        }

        // This function makes JSONObject request body.
        private JObject Body(string firstName, string lastName, string mobileNum)
        {
            JObject jsonBody = new JObject();
            jsonBody.Add("list_name", listName);

            JArray subArray = new JArray();

            JObject subscriptions = new JObject();

            JObject mobile = new JObject();
            mobile.Add("country_code", "1");
            mobile.Add("number", mobileNum);
            subscriptions.Add("mobile", mobile);

            subscriptions.Add("voice_device", "mobile");
            subscriptions.Add("first_name", firstName);
            subscriptions.Add("last_name", lastName);

            subArray.Add(subscriptions);

            jsonBody.Add("subscriptions", subArray);

            return jsonBody;
        }

        // This function retrieves report data of GET/POST response.
        private string GetReport(JObject response)
        {
            // Proper Trumpia request has status code.
            if (response["status_code"] != null)
            {
                string requestUrl = baseUrl + userName + "/report/" + response.GetValue("request_id").ToString();
                Thread.Sleep(1000);
                requestRest.requestUrl = requestUrl;
                string reportString = requestRest.Get();
                JObject report = new JObject();
                try
                {
                    report = JObject.Parse(reportString);
                }
                catch (Exception e)
                {
                    report = JObject.Parse(reportString.Substring(1, reportString.Length - 2));
                }
                if (report["subscription_id"] != null)
                {
                    if (report["message"] != null)
                        return report["message"].ToString();
                    else
                        return "Success input new Subscription";
                }
                else
                {
                    try
                    {
                        switch (report["status_code"].ToString())
                        {
                            case "MPSE2302":
                                return "Request failed - not a valid list_name.";
                            case "MPSE0501":
                                return "Request failed - Mobile number is blocked.";
                            case "MPSE2201":
                                return "Request failed - Invalid mobile number.";
                        }
                        return "Request failed - Status Code: " + report["status_code"].ToString();
                    }
                    catch (Exception e)
                    {
                        return report.ToString();
                    }
                }
            }
            // Response of Unusual request.
            return response.ToString();
        }
    }
}
