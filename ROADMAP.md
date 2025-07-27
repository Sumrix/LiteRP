# LiteRP Roadmap

## v0.1 — Bootable demo, early alpha
Basic functionality limited to launching the app and chatting.
- [x] Settings pane with **Ollama URL** and **model** configuration
- [x] Minimal chat interface (prompt ↔ response)
- [x] Lightweight file logger (`Logs/`)

## v0.2 — Basic functionality, alpha
Minimum feature set for practical usage
- [x] Navigation sidebar (Chat • Characters • Settings)
- [x] Characters page: Import TavernAI v2 `.png` files with quick previews (name + avatar)
- [x] Chat using character `description` and `first_mes` in prompts
- [x] Character deletion
- [x] Settings page moved to dedicated route
- [x] Open browser with app page on server start
- [x] Send messages using Enter key

## v0.3 — Ready to use, early beta
Feature-complete for public testing
- [ ] Chat sidebar displaying conversation history
- [ ] **Stop generation** button
- [ ] Markdown rendering in chat
- [x] Character prompts using: `personality`, `scenario`, `mes_example`
- [x] Back-end connection status indicator
- [x] Automatic settings saving
- [ ] Characters page enhancements:
  - [ ] Full character information view
  - [ ] Character **archiving**
- [ ] Compact single executable
- [ ] Hide server in system tray
- [ ] Add code signing for release builds

## v0.4 — Polished Beta
Quality of life improvements
- [ ] Global UI refresh (Flowbite/Tailwind enhancements)
- [ ] Mobile-responsive layouts
- [ ] Intuitive character card import interface
- [ ] Enhanced character catalogue:
  - [ ] List/Tile/Large-card view modes
  - [ ] Search and sorting
- [ ] Export TavernAI v2 `.png` character cards
- [ ] Right-side "Chat info" drawer (large avatar, context statistics)
- [ ] Localization support
- [ ] Reasoning model support
- [ ] Smart chat auto-scroll

## v0.5 — Feature-rich beta
Competitive feature parity with other AI frontends
- [ ] Multiple AI back-end support (ChatGPT, koboldcpp, etc.)
- [ ] Message actions: edit • regenerate • copy • swipe navigation
- [ ] Full-featured character editor
- [ ] Lorebooks page with editor
- [ ] Prompts using all character card fields
- [ ] Smart context management
- [ ] Display token count in chat

## v1.0 — Release
- [ ] Safe deletion workflow (archive browser + one-click purge)
- [ ] Configurable data folder path
- [ ] ACID-compliant file storage (DBreeze/BTDB) with snapshot backups
- [ ] Data caching
- [ ] Branching **message tree** interface
- [ ] Custom user personalities ("system profiles")
- [ ] Auto-save for unsent message drafts

## Future Plans
- [ ] llama.cpp integration
- [ ] Model management (download/delete/configure)
- [ ] LLM engine management
- [ ] Auto-update system
- [ ] Standalone installers (Windows/macOS/iOS/Android)
- [ ] Public website and documentation
- [ ] Image generation support