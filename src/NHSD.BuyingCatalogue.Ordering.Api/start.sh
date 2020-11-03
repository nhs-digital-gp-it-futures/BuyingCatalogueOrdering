#!/bin/bash
# The NHS Mail certificate has a very short key, security level drop required to send emails.
if [ "${SMTPSERVER__ALLOWINVALIDCERTIFICATE,,}" = "true" ]; 
then 
  sed -i 's|DEFAULT@SECLEVEL=2|DEFAULT@SECLEVEL=1|g' /etc/ssl/openssl.cnf; 
fi; 

dotnet NHSD.BuyingCatalogue.Ordering.Api.dll
