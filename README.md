# Playnite Play Next Addon
## What Is It?
Play Next addon for Playnite (similar to Steam PlayNext).

Since we don't have big data like Steam, the recommendations are based on your activity (playtime, recent play order) as well as general properties (like critic score, community score or release year).

![Main NextPlay view screenshot](/ci/screenshots/01.jpg)
![Main NextPlay view screenshot](/ci/screenshots/02.jpg)
The recommendation score algorithm is highly customizable to your personal taste.

## Settings
![Main NextPlay view screenshot](/ci/screenshots/03.jpg)

Total Play Next score is calculated by weighting and adding up following attributes:

| Attribute | Value |
| --------- | ---------- |
| Genre | Attribute score |
| Feature | Attribute score |
| Developer | Attribute score |
| Publisher | Attribute score |
| Tag | Attribute score |
| Critic Score | Itself |
| Community Score | Itself |
| Release Year | How close to prefered year it is |

Attribute score depends on your activity:

| Activity | Comment |
| --------- | ---------- |
| Total Playtime | All playtime |
| Recent Playtime | Playtime within recent days, requires GameActivity addon ⚠️ |
| Recent Order | Order of games played in recend days |

## Installation
You can install it either from Playnite's addon browser, or by clicking this link:[playnite://playnite/installaddon/SparrowBrain_PlayNext](playnite://playnite/installaddon/SparrowBrain_PlayNext).

## Other Addons Integration
### StartPage
![Main NextPlay view screenshot](/ci/screenshots/04.jpg)

PlayNext has a custom view for [StartPage](https://github.com/felixkmh/StartPage-for-Playnite). It's designed to look similar to StartPage game shelves.

### GameActivity
In order to get attribute score calculation based on Recent Playtime to work you will need to have [GameActivity](https://github.com/Lacro59/playnite-gameactivity-plugin) addon installed.

## Localization
You can help translate the extension to your language on the [Crowdin](https://crowdin.com/project/sparrowbrain-playnite-playnext) page.

