import time
from http.server import BaseHTTPRequestHandler, HTTPServer
from socketserver import ThreadingMixIn
import threading

HOST_NAME = 'localhost'
PORT_NUMBER = 9000


class HttpRequestHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        paths = {
            '/foo': 200,
            '/bar': 302,
            '/baz': 404,
            '/qux': 500
        }

        if self.path in paths:
            self.respond(paths[self.path])
        else:
            self.respond(404)

    def respond(self, status_code):

        if status_code != 200:
            self.send_response(status_code)
            self.end_headers()
            return

        response = self.handle_http(status_code, self.path)
        self.wfile.write(response)

    def handle_http(self, status_code, path):
        self.send_response(status_code)
        self.send_header('Content-type', 'application/json')
        self.end_headers()
        content = str({'path': path})
        return bytes(content, 'UTF-8')

class ThreadingSimpleServer(ThreadingMixIn, HTTPServer):
    pass

if __name__ == '__main__':
    server_class = ThreadingSimpleServer
    httpd = server_class((HOST_NAME, PORT_NUMBER), HttpRequestHandler)
    print(time.asctime(), 'Server Starts - %s:%s' % (HOST_NAME, PORT_NUMBER))
    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        pass
    httpd.server_close()
    print(time.asctime(), 'Server Stops - %s:%s' % (HOST_NAME, PORT_NUMBER))