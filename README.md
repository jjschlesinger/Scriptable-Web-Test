Overview
===================

Framework for running web test scripts for smoke testing websites or to generate load on a website for performance analysis. Scripts can be written in C# or a custom DSL based off of C# that makes it easier for non-programmers to write web tests.

Script Examples
===================

Example script using the DSL to do a get request to google.com and display the status code to the console

```
http.set_cookie "myCookieName" "myCookieValue"
http.set_header "accept" "text/html"
var resp = http.get "http://google.com"
#wl "Response Status: {0}" resp.status
```