﻿using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.Portico {
    [TestClass]
    public class PorticoTokenManagement {
        private string _token;


        public PorticoTokenManagement() {
            ServicesContainer.ConfigureService(new GatewayConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A",
                ServiceUrl = "https://cert.api2.heartlandportico.com"
            });

            try {
                CreditCardData card = new CreditCardData {
                    Number = "4111111111111111",
                    ExpMonth = 12,
                    ExpYear = 2015,
                    Cvn = "123"
                };
                _token = card.Tokenize();
                Assert.IsTrue(!string.IsNullOrEmpty(_token), "TOKEN COULD NOT BE GENERATED.");
            }
            catch (ApiException exc) {
                Assert.Fail(exc.Message);
            }
        }

        [TestMethod]
        public void UpdateToken() {
            var token = new CreditCardData {
                Token = _token,
                ExpMonth = 12,
                ExpYear = 2025
            };
            Assert.IsTrue(token.UpdateTokenExpiry());

            // should succeed
            var response = token.Verify().Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void DeleteToken() {
            var token = new CreditCardData {
                Token = _token
            };
            Assert.IsTrue(token.DeleteToken());

            try {
                token.Verify().Execute();
            }
            catch (GatewayException exc) {
                Assert.AreEqual("27", exc.ResponseCode);
            }
        }
    }
}
