// Countries page: switches between the 2D map and an interactive 3D globe drawn with d3-geo.
(() => {
    const wrap = document.querySelector(".map-wrap");
    if (!wrap) return;

    const views = wrap.querySelectorAll(".map-view");
    const buttons = document.querySelectorAll("[data-map-view]");
    const canvas = wrap.querySelector(".world-globe");
    const tip = wrap.querySelector(".globe-tip");
    const visited = new Set(wrap.dataset.visited.split(",").filter(Boolean));
    const reducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    let globe = null;

    // Toggle between the two views, creating the globe the first time it's shown.
    function show(view) {
        buttons.forEach(button => button.setAttribute("aria-pressed", button.dataset.mapView === view));
        views.forEach(element => element.hidden = element.dataset.view !== view);
        if (view === "3d") {
            globe ??= createGlobe();
            globe.then(g => g.start());
        } else if (globe) {
            globe.then(g => g.stop());
        }
    }

    buttons.forEach(button => button.addEventListener("click", () => show(button.dataset.mapView)));

    // Load the globe data and wire up drawing, spinning, dragging and hover.
    async function createGlobe() {
        const topo = await (await fetch(wrap.dataset.globe)).json();
        const countries = topojson.feature(topo, topo.objects.countries).features;
        const visitedCountries = { type: "FeatureCollection", features: countries.filter(f => visited.has(f.id)) };
        const markers = topojson.feature(topo, topo.objects.markers).features.filter(m => visited.has(m.id));
        const land = { type: "FeatureCollection", features: countries };
        const borders = topojson.mesh(topo, topo.objects.countries, (a, b) => a !== b);
        const graticule = d3.geoGraticule10();
        const sphere = { type: "Sphere" };

        const css = getComputedStyle(document.documentElement);
        const colour = name => css.getPropertyValue(name).trim();
        const colours = { bg: colour("--color-bg"), ocean: colour("--color-ocean"), land: colour("--color-land"), frame: colour("--color-frame"), visited: colour("--color-accent"), text: colour("--color-text") };

        const context = canvas.getContext("2d");
        const projection = d3.geoOrthographic().rotate([-10, -30]).precision(0.5);
        const path = d3.geoPath(projection, context);
        let width = 0, height = 0, frame = null, last = null, running = false;
        let spinning = !reducedMotion, drag = null, hovered = null;

        function resize() {
            const ratio = window.devicePixelRatio || 1;
            width = canvas.clientWidth;
            height = canvas.clientHeight;
            canvas.width = Math.round(width * ratio);
            canvas.height = Math.round(height * ratio);
            context.setTransform(ratio, 0, 0, ratio, 0, 0);
            projection.fitExtent([[6, 6], [width - 6, height - 6]], sphere);
            draw();
        }

        function fill(object, style) {
            context.beginPath();
            path(object);
            context.fillStyle = style;
            context.fill();
        }

        function stroke(object, style, lineWidth, alpha = 1) {
            context.beginPath();
            path(object);
            context.strokeStyle = style;
            context.lineWidth = lineWidth;
            context.globalAlpha = alpha;
            context.stroke();
            context.globalAlpha = 1;
        }

        // A point is on the visible hemisphere if it's within 90 degrees of the centre of view.
        function isVisible(coordinates) {
            const [lambda, phi] = projection.rotate();
            return d3.geoDistance(coordinates, [-lambda, -phi]) < Math.PI / 2;
        }

        function draw() {
            context.clearRect(0, 0, width, height);
            fill(sphere, colours.ocean);
            stroke(graticule, colours.frame, 0.5, 0.35);
            fill(land, colours.land);
            fill(visitedCountries, colours.visited);
            if (hovered && hovered.geometry.type !== "Point") {
                context.globalAlpha = 0.2;
                fill(hovered, colours.text);
                context.globalAlpha = 1;
            }
            stroke(borders, colours.bg, 0.6);
            for (const marker of markers) {
                if (!isVisible(marker.geometry.coordinates)) continue;
                const [x, y] = projection(marker.geometry.coordinates);
                context.beginPath();
                context.arc(x, y, marker === hovered ? 4.5 : 3.5, 0, 2 * Math.PI);
                context.fillStyle = colours.visited;
                context.fill();
                context.strokeStyle = colours.bg;
                context.lineWidth = 1;
                context.stroke();
            }
            stroke(sphere, colours.frame, 1);
        }

        function tick(time) {
            if (spinning && last !== null) {
                const [lambda, phi] = projection.rotate();
                projection.rotate([lambda + (time - last) * 0.006, phi]);
                draw();
            }
            last = time;
            frame = running ? requestAnimationFrame(tick) : null;
        }

        // Find the visited marker or country under the pointer, if any.
        function pick(x, y) {
            for (const marker of markers) {
                if (!isVisible(marker.geometry.coordinates)) continue;
                const [mx, my] = projection(marker.geometry.coordinates);
                if (Math.hypot(mx - x, my - y) < 7) return marker;
            }
            const point = projection.invert([x, y]);
            if (!point || Math.hypot(x - width / 2, y - height / 2) > projection.scale()) return null;
            return countries.find(f => f.id && d3.geoContains(f, point)) ?? null;
        }

        function pointer(event) {
            const rect = canvas.getBoundingClientRect();
            return [event.clientX - rect.left, event.clientY - rect.top];
        }

        canvas.addEventListener("pointerdown", event => {
            spinning = false;
            drag = { start: pointer(event), rotate: projection.rotate() };
            canvas.setPointerCapture(event.pointerId);
            canvas.classList.add("dragging");
        });

        canvas.addEventListener("pointermove", event => {
            const [x, y] = pointer(event);
            if (drag) {
                const sensitivity = 75 / projection.scale();
                const phi = Math.max(-90, Math.min(90, drag.rotate[1] - (y - drag.start[1]) * sensitivity));
                projection.rotate([drag.rotate[0] + (x - drag.start[0]) * sensitivity, phi]);
                tip.hidden = true;
                draw();
                return;
            }
            const found = pick(x, y);
            if (found !== hovered) {
                hovered = found;
                draw();
            }
            tip.hidden = !hovered;
            if (hovered) {
                tip.textContent = hovered.properties.name;
                tip.style.left = `${x}px`;
                tip.style.top = `${y}px`;
            }
        });

        const endDrag = () => {
            drag = null;
            canvas.classList.remove("dragging");
        };
        canvas.addEventListener("pointerup", endDrag);
        canvas.addEventListener("pointercancel", endDrag);
        canvas.addEventListener("pointerleave", () => {
            hovered = null;
            tip.hidden = true;
            draw();
        });
        window.addEventListener("resize", () => running && resize());

        return {
            start() {
                running = true;
                resize();
                last = null;
                frame ??= requestAnimationFrame(tick);
            },
            stop() {
                running = false;
                if (frame !== null) cancelAnimationFrame(frame);
                frame = null;
            }
        };
    }
})();
