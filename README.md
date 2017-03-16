# Battleship

> A Battleship game built in C# with WPF — originally a school project.

## Demo

https://github.com/user-attachments/assets/c49171e2-38e0-463b-aeab-9f8fe616a177

## About

This repository contains two versions of a Battleship game developed as a school project in 2017.

### v1 — Battaglia Navale (RETE)

The first version was built to meet the minimum requirements for an introductory programming class. It features a **network multiplayer** mode using UDP sockets, allowing two players to play against each other over a local network. The codebase is in Italian and uses a straightforward, flat class structure.

Commit tagged [`v1.0.0`](https://github.com/anomalyco/Battaglia-navale/releases/tag/v1.0.0).

### v2 — Battleship

The second version was a complete rewrite developed for two purposes:
- A **follow-up class on polymorphism**, introducing a proper `Cell` → `Water` / `Ship` class hierarchy.
- An **open-day showcase** to demonstrate student projects.

Improvements over v1 include:
- Local hot-seat multiplayer (two players on the same machine)
- Ship placement with mouse hover preview and overlap detection
- Turn indicator with color-coded UI and remaining ship counters
- Rematch functionality
- Chess-style coordinate labels (A1–J10)
- Polymorphic cell model with deep cloning support

Commit tagged [`v2.0.0`](https://github.com/anomalyco/Battaglia-navale/releases/tag/v2.0.0).

---

*This is an archived school project — no active development.*
