FROM python:3.8

COPY requirements.txt /requirements.txt
RUN pip3 install -r /requirements.txt

COPY ./src /root/src
EXPOSE 8000

WORKDIR /root
CMD [ "python3", "src/main.py" ]
