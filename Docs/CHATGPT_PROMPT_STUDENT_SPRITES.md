# ChatGPT prompt: student sprite sheets for sReader

**How to use:**
1. Start a new ChatGPT chat and attach `Docs/Student Character Sprite Sheet (1).png` as the character reference.
2. Paste **Part 1** once.
3. Then send the requests in **Part 2** one at a time: one message per sheet, and wait for each image.
4. Save each image with the file name given.

---

## Part 1: paste this first

I'm making a 2.5D reading game for children in Unity. A 2D animated student walks around a 3D classroom. I need production sprite sheets for him. The attached image is a reference for his look only. Do not copy its layout; it was too small and it changed his outfit between poses.

**The character must stay identical in every frame of every sheet:**
- A young boy, about 8 years old, in cute, glossy, rounded 3D-cartoon style (like a premium casual mobile game).
- Light, warm skin tone, rosy cheeks, large brown expressive eyes.
- Soft dark-brown curly hair. **No cap, no hat, ever.**
- Bright blue short-sleeved school polo shirt with a small white square badge on the chest.
- Navy-blue shorts, white socks, blue-and-white sneakers.
- An orange school backpack with two shoulder straps, worn on his back at all times.
- Same proportions, colours and level of detail in every frame. Only his pose and expression change.

**Technical rules for every sheet:**
- **Image size 1536 × 1024**, divided into an invisible grid of **4 columns × 2 rows**: 8 cells, each 384 wide × 512 tall. Frames read left to right, top row first.
- **One pose per cell, centred.** He stands about **440 px tall** in every cell (the same size in every frame and every sheet), with his feet on the same baseline 30 px above the bottom of the cell.
- Nothing may cross into a neighbouring cell. Keep at least 20 px of empty space around him.
- **Transparent background** (a PNG with a real alpha channel). If you can't make the background transparent, use one flat, solid **magenta #FF00FF** fill everywhere around him. Not a checkerboard, gradient, vignette, glow, floor, shadow or frame.
- **Never add a glow, halo, rim light or backlight around the character.** Light him with plain, even front lighting. Nothing coloured may touch his outline except the background colour itself.
- **No text, labels, numbers, borders, panel headers, sparkles or effects.** Only the character.
- No motion blur. Clean, crisp edges.
- An animation's frames must change smoothly from one to the next, and frame 8 must loop back to frame 1 where the animation loops.

Reply "ready" and wait for my first sheet request.

---

## Part 2: send one at a time

**1 · `student_idle_front.png`**
Idle, facing the camera, relaxed, standing still and breathing. Frames 1–8: a very subtle rise and fall of the shoulders. Eyes half-closed in frame 5 and closed in frame 6 (a blink), open again by frame 7. The same gentle closed-mouth smile in all 8 frames. Loops.

**2 · `student_idle_back.png`**
Idle, seen **from behind**, looking up at a large screen in front of him. We see the back of his curly hair and the full orange backpack. Subtle breathing; in frames 5–6 his head tilts slightly as if reading. Loops.

**3 · `student_walk_right.png`**
Walking to the right, seen from the side (facing right), backpack visible behind him. A clean 8-frame walk cycle: contact, down, passing, up, then the same with the other leg. Arms swing opposite to the legs. Loops.

**4 · `student_walk_away.png`**
Walking **away from the camera**, seen from behind (backpack and back of the hair visible), an 8-frame walk cycle with alternating legs and a small sway of the shoulders. Loops.

**5 · `student_walk_toward.png`**
Walking **toward the camera**, facing us, an 8-frame walk cycle with clearly alternating legs (knees lifting) and swinging arms. Loops.

**6 · `student_turn_to_back.png`**
Turning around on the spot, from facing the camera to facing away. Frame 1 front; 2 three-quarter front-right; 3 side (facing right); 4 three-quarter back-right; 5 back; frames 6–8 hold the back view with a small settle. Does not loop.

**7 · `student_back_point_reach.png`**
Seen **from behind**, standing at a waist-high tray of stones in front of him. Frames 1–4: raise his right arm and point forward and down at the tray, then hold. Frames 5–8: lean in slightly and reach his hand down toward the tray, then hold the reach.

**8 · `student_back_pick_place.png`**
Seen **from behind** at the same waist-high tray. Frames 1–4: pick up a small pale oval stone from the tray and lift it to chest height. Frames 5–8: move it to the side and set it gently back down on the tray, hand returning to his side.

**9 · `student_back_reactions.png`**
Seen **from three-quarter behind** (we see the backpack and part of his cheek). Row 1, frames 1–4: a happy celebration (small jump, both fists up, landing). Row 2, frames 5–8: a gentle "not quite": a small shrug, head tilting, hand rubbing the back of his head, then a little nod as he tries again. Friendly, never sad or upset.

**10 · `student_front_reactions_a.png`**
Facing the camera. Row 1, frames 1–4: celebrate (jump with arms up, big smile, land). Row 2, frames 5–8: surprised (eyebrows up, mouth "oh", small step back, return).

**11 · `student_front_reactions_b.png`**
Facing the camera. Row 1, frames 1–4: thinking (hand on chin, eyes looking up, small head tilt). Row 2, frames 5–8: gentle "not quite" (soft shrug, tilted head, encouraging half-smile), friendly and calm.

**12 · `student_read_standing.png`**
Standing, three-quarter front view, holding an open book with both hands at chest height and reading. Frames 1–8: small head movement along the lines, a page turn in frames 5–6, a smile in frame 8. Loops.

---

**After each image, check:**
- 8 poses, in a 4 × 2 layout
- the same size, with the feet on the same line
- no cap
- backpack present
- nothing crossing cell edges
- transparent or flat-magenta background
- no text

If any of these is wrong, reply: "Redo this sheet: keep the same character and fix: …"

**Walk left:** not needed. The game mirrors "walk right".
