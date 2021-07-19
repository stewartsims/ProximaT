# ProximaT
'Wheeler Dealers' episode finder

This is a very simple program I wrote to find episodes of one of my favourite TV shows 'Wheeler Dealers'. The idea was it would be useful to search for specific vehicles and fixes.

Since there was rich data on the episodes from Wikipedia I thought it would make a fun little project. I started out with a 'traditional' data indexing and search approach, converting the Wikipedia page into json files and indexing them in a basic Solr index.

However I quickly realised the amount of moving parts to do this and build (and host) a nice web front end for it would be overkill. So I wrote a tiny command line program that just naively parses the Wikipedia pages HTML on the fly and performs a proximity (fuzzy/sloppy) search.

You can see the web version here: [https://ssims.co.uk/wd-search.html](https://ssims.co.uk/wd-search.html)
Interestingly because the web version carries out a cross-origin HTTP request and the Wikipedia page is not necessarily live, I found the best way was to copy the content to my website so it runs direct from there and therefore is not regularly updated.
