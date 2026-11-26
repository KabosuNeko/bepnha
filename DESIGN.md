# Design notes - Bếp Nhà

A home-cooking recipe site for people who cook at home, with a warm kitchen feel.

## Color
- Text #2B2B2B, secondary text #6B6B6B, cream background #FAF7F2, divider lines #E7E2DA.
- One accent color only: terracotta #B4552D (clay pots, kitchen tiles). Used for primary buttons, links and the quote mark on the cook's note.
- Rating stars: dark honey #A86412 (4.67:1 contrast on white).
- Light theme only: the main content is food photos, and people often open the site in the kitchen during the day.

## Type
- Be Vietnam Pro: designed for Vietnamese diacritics, so accent marks never collide or drift.
- Headings use weight 700, no all-caps, no letter spacing.

## Layout
- Food photos are the focus; the interface stays in the background.
- Home page: search over a photo slideshow, category tiles (square photos with the name on the image), Dish of the Week as the main highlight, then a grid of the newest dishes.
- Recipe list: one dish per row, photo on the left and text on the right, so ingredients are quick to scan.
- Shared styles live in `Content/Site.css`; page-only styles live in `Content/pages/` and load through the layout's `styles` section.

## Signature detail
- The cook's note always appears as a quote, with a terracotta quote mark and italic text (recipe page, Dish of the Week, recipe cards).

## Motion
- The slideshow changes photos on its own (pauses on hover), and card and category photos zoom slightly on hover. Both effects turn off when the device has reduced motion enabled.
