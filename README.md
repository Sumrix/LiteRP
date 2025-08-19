<div align="center">
  <img src="./.github/assets/logo.png" alt="LiteRP Logo" width="300"/>
</div>

<p align="center">
  Minimal web client for chatting and roleplay with AI characters
</p>

<p align="center">
  <a href="./SCREENSHOTS.md">
    <img src="./.github/assets/screenshots/chat-light.png" alt="Chat screenshot" width="650"/>
  </a><br/>
  <sub>Chat page (light mode) – see more in the <a href="./SCREENSHOTS.md">Screenshots Gallery</a></sub>
</p>

## Features
- Ollama backend support (other APIs and backends planned)
- TavernAI v2 character cards
- No external dependencies like Python
- Single compact executable (~17 MB)

The app is under active development.  
Current version: **v0.3 Early Beta**.  
See the [ROADMAP](./ROADMAP.md) for upcoming features.

## Quick Start
- Download the executable for your platform from the [latest release](https://github.com/Sumrix/LiteRP/releases/latest).  
- **Windows**: run `LiteRP-win-x64.exe`.  
  If you see a warning about an unknown publisher, click *Run anyway*.  
- **Linux/macOS**: before first run, make the file executable:  
```bash
  chmod +x LiteRP-linux-x64
```

* On startup, LiteRP will try to open your browser automatically at <http://localhost:5000/>.
  If it doesn’t, open that URL manually.

## Contributing
Issues and PRs are welcome.  
Please check the [ROADMAP](./ROADMAP.md) before opening feature requests.

## License
[MIT](./LICENSE.txt)

## Acknowledgment
LiteRP uses or includes code from the following open source projects:

- [Flowbite Blazor](https://github.com/themesberg/flowbite-blazor) – UI components (MIT)
- [Markdig](https://github.com/xoofx/markdig) – Markdown processor (BSD-2-Clause)
- [Serilog](https://github.com/serilog/serilog) – Logging library (Apache-2.0)
- [Tailwind.MSBuild](https://github.com/MJRasicci/Tailwind.MSBuild) – Tailwind CSS build integration (MIT)
- [git-cliff-action](https://github.com/orhun/git-cliff-action) – Changelog generation (GPL-3.0)
- [action-gh-release](https://github.com/softprops/action-gh-release) – GitHub Releases publishing (MIT)
