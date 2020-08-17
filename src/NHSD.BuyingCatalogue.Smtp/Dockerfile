FROM maildev/maildev

RUN  apk add stunnel openssl

COPY stunnel.conf /etc/stunnel

RUN openssl genrsa -out key.pem 2048 && \
yes YE | openssl req -new -x509 -key key.pem -out cert.pem -days 1095  && \
cat key.pem cert.pem >> /etc/stunnel/stunnel.pem 

EXPOSE 25 80 587
ENTRYPOINT stunnel /etc/stunnel/stunnel.conf && bin/maildev -w 80 -s 25 --base-pathname /email
