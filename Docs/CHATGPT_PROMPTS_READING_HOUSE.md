# ChatGPT prompts — Reading House

Two prompts to paste into ChatGPT (image generation). Each says what the image is for, so the result drops
straight into the game.

---

## 1. The world outside the house (backdrop panorama)

**Where it goes:** save the image as `Assets/Art/ReadingHouse/Backdrop/T_WorldBackdrop.png` (replace the
placeholder), then in Unity run *Tools ▸ sReader ▸ Reading House ▸ Set Up World Backdrop*, or just press Play.
It wraps round the house on a wide curved backdrop, so the **middle of the picture sits straight behind the
house** and the two ends wrap round the sides.

> Paint a wide panoramic landscape for the background of a children's reading game, **4:1 aspect ratio
> (for example 4096 × 1024)**, seamless-looking at both ends. Style: warm, hand-painted storybook illustration,
> soft gouache/watercolour texture, cosy and magical, bright sunny late morning. Colour palette: sky blue
> fading to warm cream at the horizon, fresh spring greens, honey-gold sunlight, touches of lilac and coral.
>
> Layout: the **horizon sits about one third of the way up** the image. Foreground strip (bottom 15%): soft
> grassy meadow with small wildflowers, no hard objects. Middle distance: gentle rolling hills, a winding
> path, clusters of round friendly trees, a far-off village with a few storybook rooftops, a little
> windmill, a distant castle-like library tower, a glinting river. Far distance: pale blue mountains. Sky (top
> half): big soft fluffy clouds, a few birds, maybe a faint rainbow on one side, and paper kites flying.
> Hidden reading touches: a hot-air balloon shaped like an open book, floating letters drifting like dandelion
> seeds.
>
> **Keep the centre third calm and lower in detail** (the house stands in front of it). No people, no text,
> no logos, no UI, no frame or border, no vignette, no strong single light direction (soft even daylight).
> It must not look dark, gloomy or photo-realistic.

Tips: if the result is narrower than 4:1, ask *"extend the image left and right to 4:1, continuing the
landscape"*. Ask for a version without birds and kites if the motion looks busy.

---

## 2. Landing page for new visitors (based on the home screen)

**Attach:** a screenshot of the current student home screen (the chalkboard page) so ChatGPT keeps the
app's look.

> I'm attaching the home screen of **sReader**, a reading app for school learners in which children read
> their class assignments inside a cosy 3D "Reading House" with a friendly Encourager and a funny Boredom
> Zombie, restore key words on candy-coloured word stones, and earn points.
>
> Design a **full landing page** for new visitors (parents, teachers, schools and learners), as a tall
> single-page website mock-up, desktop width 1440 px, plus a matching mobile version (390 px wide). Keep the
> attached screen's colours and friendliness, but make it feel like a **game world, not a corporate site**:
> warm storybook illustration, soft gradients, rounded shapes, candy accents (pink, mint, lemon, cream), deep
> navy and gold for contrast.
>
> Sections, top to bottom:
> 1. **Hero**: the Reading House glowing in a sunny storybook landscape; headline *"Every child can love
>    reading"*; one-line subtitle about reading real class assignments as a game; buttons **Start reading**
>    (primary) and **I'm a teacher** (secondary); small "Log in" at the top right.
> 2. **How it works**: three illustrated steps: *Pick your assignment on the chalkboard*, *Read sentence by
>    sentence and find the key words on the stones*, *Finish, earn points and level up*.
> 3. **Meet the characters**: the Encourager (cheerful boy in a blue cap, speech bubble "You can do it!") and
>    the Boredom Zombie (sleepy, yawning, "Nooo, not reading…") with a playful line about beating boredom.
> 4. **For teachers and schools**: assign readings, see progress and hint use, academies and classes, works
>    on low-cost Android phones.
> 5. **About us**: a short warm paragraph: sReader is built to help learners read with understanding,
>    starting in Botswana, made by educators and game makers. Include a small team/illustration area.
> 6. **Contact us**: a friendly form (name, email, school, message), an email address placeholder
>    (hello@sreader.example), and social icons.
> 7. **Footer**: logo, links (About, Contact, Privacy, Terms), "Made with ❤ for young readers".
>
> Text must be large, high-contrast and readable; buttons at least 48 px tall. No stock photos of real people.
> Deliver the desktop and mobile mock-ups as images, then a short style guide (colours with hex codes, font
> suggestions, corner radius, button styles) so it can be rebuilt in Unity UI Toolkit.
