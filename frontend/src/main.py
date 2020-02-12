#!/usr/bin/env python3
import os
from flask import Flask

app = Flask(__name__)


@app.route('/')
def url_home():
    return '<html><body><h1>Hello World</h1></body></html>'


if __name__ == '__main__':
    app.run(host="0.0.0.0", port=os.getenv("PORT"))
