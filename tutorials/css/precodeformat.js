// Copyright 2008 Gary Kacmarcik
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// The latest version of this code lives at:
//   http://code.google.com/p/precodeformat/

(function() {
	// Define our regex patterns.
	var regex_Space = / /g;
	var regex_Newline = /^(\r\n?|\n)/;
	var regex_Tab = /^\t/;
	var regex_Comment = /^(\/\/.*?)(\r\n?|\n)/;
	var regex_CommentMulti = /^\/\*.*\*\//;
	var regex_Disabled = /^(<disabled\s*\/?>.*)(\r\n?|\n)/i;
	var regex_Mark = /^<mark type="(\w+)"\s*\/?>/i;
	var regex_CloseTag = /^<\/(mark|disabled)>/i;
	var regex_String = /^("|').*\1/;
	var regex_Token = /^\w+/;

	// True if we need to add special start-of-line tags before the next tag.
	var _needs_startline;
	
	// Keep track of the number of open <spans> so that we can close them
	// all out at the end of the line.
	var _num_open_spans;

	// Browser hacks and workarounds.
	var _needs_nbsp_for_blank_lines = false;
	
	if (navigator && navigator.userAgent) {
		if (/MSIE 7/.test(navigator.userAgent)) {
			_needs_nbsp_for_blank_lines = true;
		}
	}
	
	// Default keywords for C/C++.
	var DefaultKeywords =	""
		// Basic C keywords
		+ "auto break case char const continue default do double else enum extern float "
		+ "for goto if int long register return short signed sizeof static struct switch "
		+ "typedef union unsigned void volatile while"
		// C++ keywords
		+ "asm bool catch class const_cast delete dynamic_cast explicit export false "
		+ "friend inline mutable namespace new operator private protected public "
		+ "reinterpret_cast static_cast template this throw true try typeid typename "
		+ "using virtual wchar_t"
		+ "";

	function GetSymbols(symlist_string) {
		var sym = {};
		if (symlist_string) {
			var symlist = symlist_string.split(regex_Space);
			for (var i = symlist.length; --i >= 0;) {
				var w = symlist[i];
				if (w) {
					sym[w] = null;
				}
			}
		}
		return sym;
	}

	// Close out the current line.
	// Return a string that closes out all open <span> tags.
	function EndOfLine() {
		var close_spans = [];
		while (_num_open_spans > 0) {
			close_spans.push("</span>");
			_num_open_spans--;
		}
		
		_needs_startline = true;
		return close_spans.join('');
	}
	
	// Process a single <pre class="code"> element.
	function ProcessPreCodeElement(elem, keywords) {
		var variables = GetSymbols(elem.getAttribute('variables'));
		var functions = GetSymbols(elem.getAttribute('functions'));
		var extra_keywords = GetSymbols(elem.getAttribute('keywords'));
		
		var ugly = elem.innerHTML;
		var pretty = [];
		
		// Have we found a tag so far in this <pre> block?
		var found_tag = false;
		// Have we found a non-whitespace token yet on this line?
		var found_nonws_line = false;

		_needs_startline = true;
		_num_open_spans = 0;
		
		// Process each token in the ugly HTML and add beautifying decorations
		// as appropriate.
		for (; ugly.length; ugly = ugly.substring(token.length)) {
			var token = null;
			var m;

			// Detect code lines that should be marked.
			// These tags must be the first tag on the line since they will replace
			// the default 'startline'.
			m = ugly.match(regex_Mark);
			if (m) {
				token = m[0];
				style = m[1];
				pretty.push('<span class="startline ' + style + '">');
				_num_open_spans++;
				_needs_startline = false;
				// Set found_tag to true to handle the case where this is the very
				// first tag in the <pre>.
				found_tag = true;
				found_nonws_line = false;
				continue;
			}
			// Ignore extra closing tags.
			// Remove these before handling _needs_startline so that extra </mark> and
			// </disabled> tags inserted by the browser don't result in an extra line
			// at the end of the <pre>.
			m = ugly.match(regex_CloseTag);
			if (m) {
				token = m[0];
				continue;
			}

			// Add the startline tag if needed.
			if (_needs_startline) {
				pretty.push('<span class="startline">');
				_num_open_spans++;
				_needs_startline = false;
				found_tag = true;
				found_nonws_line = false;
			}

			// Convert newlines to <br/> tags.
			m = ugly.match(regex_Newline);
			if (m) {
				token = m[0];
				// Ignore newlines until we find our first real tag.
				if (found_tag) {
					if (!found_nonws_line && _needs_nbsp_for_blank_lines)
						pretty.push('&nbsp;');
					pretty.push('<br/>');
					pretty.push(EndOfLine());
				}
				continue;
			}

			// Expand tabs to spaces.
			m = ugly.match(regex_Tab);
			if (m) {
				token = m[0];
				pretty.push('&nbsp;&nbsp;&nbsp;&nbsp;');
				found_tag = true;
				continue;
			}
			// Detect comments.
			m = ugly.match(regex_Comment);
			if (m) {
				token = m[0];
				pretty.push('<span class="comment">');
				pretty.push(m[1]);	// Don't include newline
				pretty.push('</span>');
				pretty.push('<br/>');
				pretty.push(EndOfLine());
				found_tag = true;
				found_nonws_line = true;
				continue;
			}
			// Detect multi-line comments.
			m = ugly.match(regex_CommentMulti);
			if (m) {
				token = m[0];
				pretty.push('<span class="comment">');
				pretty.push(token);
				pretty.push('</span>');
				found_tag = true;
				found_nonws_line = true;
				continue;
			}
			// Detect 'disabled' code.
			// All code from the tag to the EOL is 'disabled' (i.e., unhilighted) and it will
			// not have pretty formatting applied.
			// If the line is indented, be sure to place the <disabled/> tag *after* the
			// tab or else the tab will not be expanded into spaces properly.
			m = ugly.match(regex_Disabled);
			if (m) {
				token = m[0];
				pretty.push('<span class="disabled">');
				pretty.push(m[1]);	// Don't include newline
				pretty.push('</span>');
				pretty.push('<br/>');
				pretty.push(EndOfLine());
				found_tag = true;
				found_nonws_line = true;
				continue;
			}
			// Detect strings.
			m = ugly.match(regex_String);
			if (m) {
				token = m[0];
				pretty.push('<span class="string">');
				pretty.push(token);
				pretty.push('</span>');
				found_tag = true;
				found_nonws_line = true;
				continue;
			}

			// Match all other tokens.
			m = ugly.match(regex_Token);
			if (m) {
				token = m[0];
			}
			found_tag = true;
			found_nonws_line = true;

			// Make sure that always match at least one char.
			if (!token) {
				token = ugly.substring(0, 1);
			}

			// Add any decorations to pretty up the token.
			var has_span = false;
			if (token in keywords || token in extra_keywords) {
				pretty.push('<span class="keyword">');
				has_span = true;
			}
			if (token in variables) {
				pretty.push('<span class="variable">');
				has_span = true;
			}
			if (token in functions) {
				pretty.push('<span class="function">');
				has_span = true;
			}

			// Add the (ugly) token.
			pretty.push(token);

			// Close out any pretty decorations.
			if (has_span) {
				pretty.push('</span>');
			}
		}

		elem.innerHTML = pretty.join('');
	}

	// Format all <pre class="code"> tags.
	function PreCodeFormat() {
		var keywords = GetSymbols(DefaultKeywords);
	
		var preElements = document.getElementsByTagName('pre');
		for (var i = 0; i < preElements.length; ++i) {
			var elem = preElements[i];
			if (elem.className && elem.className.indexOf('code') >= 0) {
				ProcessPreCodeElement(elem, keywords);
			}
		}
		preElements = null;
	}

	function ExportSymbol(name, value) {
		window[name] = value;
	}

	ExportSymbol('PreCodeFormat', PreCodeFormat);
})();
