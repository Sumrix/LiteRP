# LiteRP
> Lightweight LLM frontend for roleplay—Blazor Server, zero install, just run.

⚠️ **v0.2 Alpha** – “Basic Functionality”, ~100 MB, not yet a single file.

### Key features
- Configure **Ollama** URL & model
- **Characters page**: import/delete TavernAI v2 `.png` cards
- **Chat** with character context (`description` + `first_mes`)

See the [ROADMAP](./ROADMAP.md) for what’s coming next.

## Quick start
1. Download the [latest release](https://github.com/Sumrix/LiteRP/releases/latest).
2. Extract the folder.
3. Run `LiteRP.exe`.  
   • On first run Windows may warn about an unknown publisher – that’ll disappear once code‑signing lands in v0.3.
4. Your default browser should open automatically at <http://localhost:5000>.  
   If it doesn’t, open that URL manually.
5. Open settings page and configure Ollama URL and model name, then save.

> Ollama must be running locally (or reachable via URL) and the model you choose must be pulled beforehand.

## Contributing
Issues and PRs welcome. Please check the [ROADMAP](./ROADMAP.md) first.

## License
[MIT](./LICENSE.txt)