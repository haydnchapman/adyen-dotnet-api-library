﻿using Adyen.EcommLibrary.Constants;
using Adyen.EcommLibrary.Constants.HPPConstants;
using Adyen.EcommLibrary.Model.Hpp;
using Adyen.EcommLibrary.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Adyen.EcommLibrary.Test
{
    [TestClass]
    public class DirectoryLookupTest : BaseTest
    {
        [TestMethod]
        public void TestGetPostParameters()
        {
            var client = base.CreateMockTestClientRequest("");
            var hostedPaymentPages = new HostedPaymentPages(client);
            var directoryLookupRequest = CreateDirectoryLookupRequest();
            var postParameters = hostedPaymentPages.GetPostParametersFromDlRequest(directoryLookupRequest);

            Assert.AreEqual("EUR", postParameters[Fields.CurrencyCode]);
            Assert.AreEqual(44, postParameters[Fields.MerchantSig].Length);

        }

        [TestMethod]
        public void TestGetPaymentMethods()
        {
            var client = base.CreateMockTestClientPost("Mocks/hpp/directoryLookup-success.json");
            var hostedPaymentPages = new HostedPaymentPages(client);
            var directoryLookupRequest = CreateDirectoryLookupRequest();
            var paymentMethods = hostedPaymentPages.GetPaymentMethods(directoryLookupRequest);

            Assert.AreEqual(8, paymentMethods.Count);
            //Get payment method by name
            var ideal = paymentMethods.FirstOrDefault(x => x.Name == "iDEAL");
            //  Assert.IsFalse(ideal.IsCard());

            Assert.AreEqual(BrandCodes.Ideal, ideal.BrandCode);
            Assert.AreEqual("iDEAL", ideal.Name);
            Assert.AreEqual(3, ideal.Issuers.Count);
            var issuerIdeal = ideal.Issuers.FirstOrDefault();

            Assert.AreEqual("1121", issuerIdeal.IssuerId);
            Assert.AreEqual("Test Issuer", issuerIdeal.Name);

            //Get payment method by name
            var visa = paymentMethods.FirstOrDefault(x => x.Name == "VISA");
            Assert.AreEqual(BrandCodes.Visa, visa.BrandCode);

            //Assert.IsTrue(visa.IsCard());
        }

        [TestMethod]
        public void TestGetPaymentMethodsError()
        {
            try
            {
                var client = base.CreateMockTestClientPost("Mocks/hpp/directoryLookup-error.htm");
                var hostedPaymentPages = new HostedPaymentPages(client);
                var directoryLookupRequest = CreateDirectoryLookupRequest();
                Assert.Fail("Expected exception");
            }
            catch (Exception)
            {
                
            }
        }
        
        private DirectoryLookupRequest CreateDirectoryLookupRequest()
        {
            DirectoryLookupRequest directoryLookupRequest = new DirectoryLookupRequest()
            {
                CountryCode = "NL",
                MerchantReference = "test:\\'test",
                PaymentAmount = "1000",
                CurrencyCode = "EUR"
            };
            return directoryLookupRequest;
        }
    }
}
