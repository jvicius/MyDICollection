# Third-Party Notices

MyDICollection is an independent, community-developed project. This file
documents third-party projects and publicly available interoperability
research consulted during development.

Inclusion of a notice here does not imply affiliation, sponsorship,
endorsement, or ownership by any third party.

## Proxmark3 / RfidResearchGroup

**Project:** Proxmark3  
**Repository:** https://github.com/RfidResearchGroup/proxmark3  
**License:** GNU General Public License v3.0 or later (GPL-3.0-or-later)  
**Copyright:** Proxmark3 contributors. Individual contributions remain
attributable to their respective authors.

During development of MyDICollection's read-only Disney Infinity NFC
identification functionality, publicly available interoperability research
from the Proxmark3 project was consulted.

This included historical Lua tooling related to Disney Infinity and
MIFARE Mini data handling, historically named `didump.lua` and later
renamed within the Proxmark3 project history.

MyDICollection does not bundle, execute, link against, or redistribute
the Proxmark3 application or its Lua scripts.

MyDICollection implements its NFC identification workflow in C# using
the platform cryptographic and NFC APIs. Protocol behavior and constants
necessary for interoperability that were learned from publicly available
research are acknowledged here for provenance and transparency.

Proxmark3 is distributed under the GNU General Public License v3.0 or
later. The authoritative license terms, copyright information, and
project history are available from the upstream Proxmark3 repository.

This notice provides attribution and provenance. It does not make a
legal determination regarding the applicability of any third-party
license to independently copyrightable portions of MyDICollection and
does not constitute legal advice.

## Disney and related properties

Disney Infinity, Disney, Pixar, Marvel, Star Wars, and related names,
characters, logos, trademarks, and other intellectual property are the
property of their respective owners.

MyDICollection is an independent collection-management project and is
not affiliated with, endorsed by, sponsored by, or associated with
The Walt Disney Company or its affiliates.

References to Disney Infinity and related properties are used solely
to identify compatible collectible items and support personal collection
management.

MyDICollection's public NFC functionality is intended for read-only
identification of physical collectible items. The project does not
provide functionality for NFC tag cloning, emulation, or writing.