PePayload
=============

About
-----

PePayload appends payload to signed binaries without break signing. There are a lot of implementations in different languages. Some do work. Some don't.
This is a working implementation in C#.

What it does
------------

While appending junk bytes to unssigned binaries does not affect loading , appending bytes to signed binaries does.
The loader assumes the last part of a PE binary to be a certificate table. This table is defined in "Data Directories" section in the "Optional Header" with address and length.
However, signing validation fails when length changes.

PePayload appends a string of your choice to signed PE files considering the changing length of the certificate table mentioned in the data directory as well as in the certifcate
table itself.

How to use
----------

Just use Payload.Append(inputFile, outputFile, payload) where payload is your payload.
Your payload should use some magics as start and end markers. Some use notations like <#$@@$#>...</#$@@$#>

To retrieve your payload just read the binary as text and use regular expression to find your payload.

Do not use a standard xml preamble as start marker. You would get the manifest (if any) too.

Licensing
---------

PePayload is Open Source under GPLv3.