﻿/*
 * Twix v1.0 - a lightweight library for making AJAX requests.
 * Author: Neil Cowburn (neilco@gmail.com)
 * 
 * Copyright (c) 2013 Neil Cowburn (http://github.com/neilco/)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
var Twix = function () { function t() { } return t.ajax = function (t) { t = t || { url: "" }, t.type = t.type || "GET", t.headers = t.headers || {}, t.timeout = parseInt(t.timeout) || 0, t.success = t.success || function () { }, t.error = t.error || function () { }, t.async = "undefined" == typeof t.async ? !0 : t.async; var e = new XMLHttpRequest; t.timeout > 0 && (e.timeout = t.timeout, e.ontimeout = function () { t.error("timeout", "timeout", e) }), e.open(t.type, t.url, t.async); for (var s in t.headers) t.headers.hasOwnProperty(s) && e.setRequestHeader(s, t.headers[s]); return e.send(t.data), e.onreadystatechange = function () { if (4 == this.readyState && 200 == this.status) { var e = this.responseText; this.getResponseHeader("Content-Type").match(/json/) && (e = JSON.parse(this.responseText)), t.success(e, this.statusText, this) } else 4 == this.readyState && t.error(this.status, this.statusText, this) }, 0 == t.async && (4 == e.readyState && 200 == e.status ? t.success(e.responseText, e) : 4 == e.readyState && t.error(e.status, e.statusText, e)), e }, t.get = function (e, s, r) { return "function" == typeof s && (r = s, s = void 0), t.ajax({ url: e, data: s, success: r }) }, t.post = function (e, s, r) { return "function" == typeof s && (r = s, s = void 0), t.ajax({ url: e, type: "POST", data: s, success: r }) }, t }(); __ = Twix;