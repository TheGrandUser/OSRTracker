# OSR Tracker - Product Brief

**Version:** 1.0 | **Date:** 2026-05-20 | **Author:** Andrew W. Johnson

## Overview

This application helps the Game Master keep track of characters, treasure, and XP rewards by session and dungeon delve. It can auto-calculate character XP and levels and XP requirements. It can also keep track of the other basic attributes of a character.

## Target Users
- Game Masters
- Potentially players

## Core Features (MVP)
- [ ] Character Roster
- [ ] Session / Delve Tracker
- [ ] XP Chart
- [ ] Treasure List

## Non-Functional Requirements
- Platforms: Windows / macOS / Linux (Electron / Tauri / Native)
- Performance: Open a project with 10k records in < 2 seconds
- Offline: Fully offline-first
- Data: Single .sqlite file per project (or default to `project.db`)
- Auto-save: Every change is persisted immediately / on timer
- File format: .myapp extension (contains SQLite + optional attachments)

## Success Criteria

### MVP Success Criteria
- Users can create a campaign, add characters, log a session with multiple treasure items (with apparent/actual values), and correctly calculate XP.
- All core treasure fields (apparent value, actual value, identification status) function reliably.
- Character XP and treasure totals update accurately after allocation.
- Basic reports and history views display correct data.
- App is usable for a full evening session without major friction.

### User Experience Success
- Average time to log a typical delve with 8–10 treasure items < 4 minutes.
- No manual math required for standard 1gp = 1xp calculations.
- Clear visual distinction between identified and unidentified items.
- Mobile-friendly interface suitable for use at the table.

### Functional Success
- 100% accuracy in XP and treasure total calculations.
- Data persistence — no loss of sessions, characters, or treasure entries.
- Ability to retroactively identify items and see updated totals/XP.
- Support for at least 3 simultaneous campaigns with 6+ characters each.

### Long-term Success Metrics
- Users can track a full campaign (10+ sessions) without performance issues.
- Positive feedback on how well the app captures the "mystery and discovery" feeling of OSR treasure.
- Less than 5% of logged items need correction after initial entry.
- Export functionality produces usable, clean character sheets.

### Technical Success
- Fast loading of campaign history and reports

# User Stories

**User Stories for OSR Treasure & XP Tracker App**

### Core Setup & Campaign Management

**As a Game Master (GM),**  
I want to create a new campaign  
so that I can organize all sessions, characters, and loot for a specific group/adventure.

**As a GM,**  
I want to add player characters (with name, class, level, and optional notes) to a campaign  
so that I can track XP and treasure per character.

**As a GM or Player,**  
I want to view a list of all my campaigns with quick stats (total sessions, total XP earned, total treasure value)  
so that I can easily switch between games.

### Session & Dungeon Delve Logging

**As a GM,**  
I want to log a new Session with date, session title/notes, and overall XP & treasure awarded  
so that I have a permanent record of what happened.

**As a GM,**  
I want to create a Dungeon Delve inside a session (or as a standalone entry) with its own name, depth/level, and specific treasure/XP  
so that I can differentiate between different dungeon runs or side adventures.

**As a GM,**  
I want to record individual treasure items with the following fields:  
- Name/Description  
- Apparent Value (what it appears to be worth at first glance)  
- Actual Value (true value after identification or appraisal)  
- Identified status (Yes/No/Partially)  
- Notes (e.g., "found in trapped chest", "radiates magic")  
so that I can track both the uncertainty of treasure and its real worth.

**As a GM,**  
I want the app to show both apparent total and actual total value for a delve or session  
so that I can see the difference between surface appearance and true value of loot.

**As a GM,**  
I want to mark an item as "identified" later and update its actual value  
so that I can reflect in-game identification (by spell, expert, or time) without losing the original apparent value.

**As a GM,**  
I want the app to automatically calculate XP from treasure using the actual value (classic 1 gp = 1 XP or custom multiplier) once identified, or use apparent value as a temporary estimate  
so that XP awards remain accurate to the rules while supporting the mystery of unidentified loot.

### Character Progress Tracking

**As a GM or Player,**  
I want to assign XP and treasure shares to specific characters after a session/delving (including shares of both apparent and actual value) so that each PC has accurate personal totals.

**As a Player,**  
I want to see my character's current total XP, current level, XP needed for next level, lifetime treasure acquired (apparent + actual), and list of unidentified items 
so that I know exactly where I stand.

**As a GM,**  
I want to mark treasure as "divided among party", "claimed by specific PC", or "kept in party fund" 
so that I can track both individual wealth and shared resources accurately.

### History, Reports & Insights

**As a GM or Player,**  
I want to view a chronological history of all sessions and delves with filters (by date, by dungeon, by character, by identified status)  
so that I can review past adventures.

**As a GM,**  
I want a summary report for a session or campaign showing:  
- Total XP awarded  
- Total apparent treasure value  
- Total actual treasure value  
- Number of unidentified items remaining  
- Treasure per character  
so that I can analyze pacing and balance.

**As a GM,**  
I want to generate a simple character sheet export (PDF or text) with current level, XP total, notable treasures, and identification status of carried items  
so that I can share updates easily with players.

### Additional Helpful Features

**As a GM,**  
I want to apply house rules for XP (e.g., multipliers for roleplaying, penalties for certain actions, or different gp-to-xp ratios)  
so that the app matches my table's specific old-school variant.

**As a GM,**  
I want to log monster kills or special awards with manual XP values  
so that I have full flexibility beyond pure gold-for-XP.

**As a Player or GM,**  
I want to attach photos or detailed notes to treasure entries  
so that memorable loot has context and flavor.

**As a GM,**  
I want the ability to "retcon" or edit past entries (with a visible edit history)  
so that mistakes in recording can be fixed without losing data integrity.