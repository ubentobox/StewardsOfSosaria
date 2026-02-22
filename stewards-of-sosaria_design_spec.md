# Steward's of Sosaria
## Town Management & NPC Command System for Ultima-Adventures

---

# 1) Core Pillars and Modes

## Town Ownership Models
- Player-owned town (single governor)
- Guild-owned town (council / roles)
- Faction-controlled town (military governance)
- NPC-controlled town with player influence (soft power)

## Control Modes
- Indirect: assign tasks, set policies, provide supplies
- Direct: temporarily possess/command an NPC (with rules/limits)
- Delegated: appoint steward/captain NPCs to execute directives

## Game Loops
- Daily operations loop: food, labor, patrols, crafting, repairs
- Expansion loop: claim land → build structures → attract settlers
- Threat loop: raids, monsters, politics, disasters
- Progression loop: tech/skill upgrades, reputation, prosperity tiers

---

# 2) NPC System

## 2.1 NPC Identity & Stats
- NPC Archetype / Profession (farmer, mason, guard, healer, smith, scribe, priest, ranger, miner, etc.)
- Skills (reuse UO skills where possible; add derived skills if needed)
- Attributes (STR/DEX/INT + derived stats: stamina, carry capacity, work speed)

### Personality Traits
- Brave / Cowardly
- Honest / Greedy
- Industrious / Lazy
- Loyal / Rebellious
- Risk tolerance, aggression, sociability

### Needs & Mood
- Hunger / Thirst / Rest
- Safety
- Comfort
- Morale
- Mood modifiers: pay, food quality, recent deaths, victories, festivals

### Relationships
- Friends / Rivals / Family
- Loyalty to town, governor, guild
- Conflict system (brawls, crime, desertion, coups – optional depth)

## 2.2 NPC Autonomy & Schedules
- Daily schedule blocks: Work / Patrol / Rest / Eat / Worship / Recreation / Training
- Rituals / Events: market days, services, guard rotations, meetings
- Behavior priority stack:
  - Emergency (fire/raid)
  - Needs (eat/sleep)
  - Assigned tasks
  - Idle behaviors

## 2.3 Inventories & Ownership
- Personal inventory (true item inventory)
- Tool requirements per job
- Storage access rules:
  - Town stockpiles
  - Restricted armory / treasury
  - Private property (theft consequences)

---

# 3) Town Management Layer

## 3.1 Town Entity & Governance
- Defined town boundaries (region/tiles)
- Prosperity tier (hamlet → village → town → city)
- Law/policy framework
- Governance roles:
  - Governor / Mayor
  - Treasurer
  - Captain of the Guard
  - Quartermaster
  - Magistrate / Sheriff
  - Builder / Architect NPC

## 3.2 Policies & Law System
- Tax rates (low/normal/high)
- Rationing (generous/standard/strict)
- Curfew rules
- Justice model (warnings, fines, jail, banishment, execution)
- Crime mechanics (theft detection, contraband, bounties, prison labor)

## 3.3 Town Resources
### Physical Goods
- Wood, stone, iron
- Food, cloth
- Weapons, medicine

### Abstract Meters
- Prosperity
- Security
- Health
- Morale
- Reputation (regional/global)

### Supply Chains
- Imports / exports (caravans, ships)
- Market pricing models
- Shortage effects (theft, unrest, desertion)

---

# 4) Building & Construction

## 4.1 Claims and Zoning
- Claim land system (permissions + cost)
- Zoning types:
  - Residential
  - Agriculture
  - Industrial
  - Military
  - Civic / Religious

## 4.2 Construction System
- Blueprint/ghost placement
- Material requirements + staged builds
- Labor assignment (haulers, builders)
- Decay and maintenance
- Siege/fire damage

## 4.3 Building Categories
- Housing (tents → cottages → rowhouses)
- Food (farms, mills, bakeries, granaries)
- Industry (lumberyard, quarry, mine, forge, tannery)
- Military (barracks, armory, towers, walls, gates)
- Civic (town hall, jail, clinic, school, marketplace)
- Spiritual (shrines, temples, morale systems)

---

# 5) Task System

## 5.1 Task Types
- Gather
- Haul
- Craft
- Build/Repair
- Patrol
- Train
- Trade
- Heal
- Research
- Quest/Mission

## 5.2 Task Mechanics
- Priority levels
- Dependencies (tools, materials, skill requirements)
- Reservation system (avoid duplicate hauling)
- Failure states (blocked path, attacked, missing tools)
- Recovery behaviors (retry, request help, report failure)

## 5.3 Task UI
- Town task board
- Drag priority system
- Zone marking (cut trees, stockpiles, patrol routes)
- Delegation to captains/foremen

---

# 6) Quests, Goals & Progression

## 6.1 NPC Personal Goals
- Skill mastery
- Social goals (family/home)
- Moral or spiritual quests
- Ambition (promotion, shop ownership)

## 6.2 Town Goals
- Milestones (granary, walls, trade route)
- Era/tech progression
- Reputation ladders

## 6.3 Player Interaction
- Supply items to advance goals
- Directly control NPCs for dangerous objectives
- Sponsor training, equipment, festivals

---

# 7) Combat & Command

## 7.1 Squad System
- Form squads (militia, rangers, rescue teams)
- Tactical orders (attack, hold, fallback)
- Strategic postures (aggressive, defensive, avoid)
- Morale effects (panic, rally, rout)

## 7.2 Town Defense
- Threat detection (scouts, towers, alarms)
- Siege-lite mechanics (walls, gates, fire)
- Guard rotations and patrols

## 7.3 Threat Types
- Monster raids
- Bandits
- Rival towns (PvP shards)
- Internal unrest

---

# 8) Economy & Trade

- Vendor restocking systems
- Production chains (ore → ingots → weapons; wheat → flour → bread)
- Town treasury (taxes, wages, upkeep)
- Trade routes (scheduled caravans, ambush risk)
- Pricing models (static vs dynamic)

---

# 9) Social Systems & Population

- Immigration/recruitment mechanics
- Population caps (housing/food)
- Wages & employment
- Events (festivals, funerals, elections)

---

# 10) World Integration

- Virtue/alignment impact systems
- Diplomacy with existing towns
- Dungeon-linked safety mechanics
- Lore-based shrine and regional story arcs

---

# 11) Interface / UX

- Town Ledger Gump (resources, population, alerts)
- Task Board Gump (queue, priority, failures)
- NPC Sheet (stats, traits, needs, schedule, inventory)
- Map overlays (zones, patrols, alerts)
- Notification system (shortage, raid, injury)
- Permission controls by rank

---

# 12) Technical Mechanics

## 12.1 Persistence & Data Model
- Town data (boundaries, buildings, policies, treasury)
- Extended NPC data (traits, needs, goals, schedules)
- Persistent task queues
- Versioned serialization and migrations

## 12.2 Performance Budgeting
- Simulation throttling for distant towns
- Event-driven updates
- Pathfinding caching/limits
- Batch processing for hauling/crafting

## 12.3 Security & Abuse Prevention
- Anti-dupe via reservation system
- Controlled possession rules
- Audit logging for governance actions

---

# 13) Multiplayer Rules

- Founding requirements
- Vulnerability windows
- Offline protection rules
- Building placement restrictions
- Diplomacy tools (alliances, embargoes)
- Competitive leaderboards

---

# 14) Content Systems

- Procedural missions (escort, clear camp, rescue NPC)
- Seasonal effects (winter drain, storms, harvest)
- Tech/unlock trees (walls, farms, doctrines, civic upgrades)

---

_End of Sections 1–14_

