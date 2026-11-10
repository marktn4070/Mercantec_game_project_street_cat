let gotBoost = false;
let boostRequestInProgress = false;
let useBoostRequestInProgress = false;
let leftKeyWasDown = false;
let gameStarted = false;
let timerInterval = null;
let getDataInterval = null;
let decisecond = 0;
let lastDecisecond = 0;
let gameSession = 0;
let playerMode = "";

let remoteJump = false;
let remoteCrawl = false;
console.log("GAME FILE LOADED");

//Call restartgame
function resetGame() {
	gotBoost = false;
	boostRequestInProgress = false;
	useBoostRequestInProgress = false;
	leftKeyWasDown = false;
	gameStarted = false;
	decisecond = 0;
	playerMode = "";
}

function stopCurrentGame() {
    gameSession++;
	openedPopup = true;
	gameStarted = false;

	if (timerInterval !== null) stopTimer();
	if (getDataInterval !== null) stopDataInterval();

	if (decisecond > lastDecisecond){
		lastDecisecond = decisecond;
	}
	resetGame();
}

class Vec {
    constructor(x, y) {
        this.x = x;
        this.y = y
    }
    plus(other) {
        return new Vec(this.x + other.x, this.y + other.y)
    }
    times(factor) {
        return new Vec(this.x * factor, this.y * factor)
    }
}
class Level {
    constructor(plan) {
        let rows = plan.trim().split("\n").map(l => [...l]);
        this.height = rows.length;
        this.width = rows[0].length;
        this.startActors = [];
        this.rows = rows.map((row, y) => row.map((ch, x) => {
            let type = levelChars[ch];
            if (typeof type != "string") {
                let pos = new Vec(x, y);
                this.startActors.push(type.create(pos, ch));
                type = "empty"
            }
            return type
        }))
    }
    touches(pos, size, type) {
        let xStart = Math.floor(pos.x),
            xEnd = Math.ceil(pos.x + size.x),
            yStart = Math.floor(pos.y),
            yEnd = Math.ceil(pos.y + size.y);
        for (let y = yStart; y < yEnd; y++)
            for (let x = xStart; x < xEnd; x++) {
                let outside = x < 0 || x >= this.width || y < 0 || y >= this.height;
                let here = outside ? "wall" : this.rows[y][x];
                if (here == type) return true
            }
        return false
    }
}
class State {
    constructor(level, actors, status) {
        this.level = level;
        this.actors = actors;
        this.status = status
    }
    static start(level) {
        return new State(level, level.startActors, "playing")
    }
    get player() {
        return this.actors.find(a => a.type == "player")
    }
    update(time, keys) {
        let actors = this.actors.map(actor => actor.update(time, this, keys));
        let state = new State(this.level, actors, this.status);
        if (state.status != "playing") return state;
        let player = state.player;
        if (this.level.touches(player.pos, player.size, "lava")) return new State(this.level, actors, "lost");
        for (let actor of actors)
            if (actor != player && overlap(actor, player)) state = actor.collide(state);
        return state
    }
}

class Player {
    constructor(pos, speed, size = new Vec(.8, 1), animation = "standStill") {
        this.pos = pos;
        this.speed = speed;
        this.size = size;
        this.animation = animation;
    }

    get type() {
        return "player";
    }

	static create(pos) {
		return new Player(
			pos.plus(new Vec(0, -.5)),
			new Vec(0, 0),
			new Vec(.8, 1),
			"standStill"
		);
	}

	update(time, state, keys) {
		let xSpeed = 0;

		let animation = gameStarted ? "player" : "standStill";

		if (keys.ArrowDown || remoteCrawl)
            animation = "crawl";

		if (playerMode === "Invisibility") {
			animation += " Invisibility";
		}else{
			animation = animation;
		}

		if (gameStarted) xSpeed = 7;

        const leftKeyPressed = keys.ArrowLeft && !leftKeyWasDown;

        leftKeyWasDown = keys.ArrowLeft;

        if (leftKeyPressed) {
            if (!gotBoost) {
				playerMode = "Invisibility";
                postBoostName(playerMode);
            } else {
                postBoostUse();
            }
        }

        let normalSize = new Vec(.8, 1);
        let crouchSize = new Vec(.8, .75);

        let pos = this.pos;
        let size =
            (keys.ArrowDown || remoteCrawl)
                ? crouchSize
                : normalSize;

        if (!keys.ArrowDown && this.size.y < normalSize.y) {
            let standingPos = pos.plus(
                new Vec(0, this.size.y - normalSize.y)
            );

            if (!state.level.touches(standingPos, normalSize, "wall")) {
                pos = standingPos;
            } else {
                size = crouchSize;
            }
        }

        let movedX = pos.plus(new Vec(xSpeed * time, 0));

        if (!state.level.touches(movedX, size, "wall")) {
            pos = movedX;
        }

        let ySpeed = this.speed.y + time * 30;
        let movedY = pos.plus(new Vec(0, ySpeed * time));

        if (!state.level.touches(movedY, size, "wall")) {
            pos = movedY;
        } else if ((keys.ArrowUp || remoteJump) && ySpeed > 0) {
            console.log(
                "JUMP TRIGGERED",
                "remoteJump =", remoteJump,
                "ArrowUp =", keys.ArrowUp
            );
            ySpeed = -12;
            remoteJump = false;
        } else {
            ySpeed = 0;
        }
        
		return new Player(
			pos,
			new Vec(xSpeed, ySpeed),
			size,
			animation
		);
    }
}

class Lava {
    constructor(pos, speed, reset) {
        this.pos = pos;
        this.speed = speed;
        this.reset = reset
    }
    get type() {
        return "lava"
    }
    static create(pos, ch) {
		if (ch == "=") {
			return new Lava(
				pos.plus(new Vec(0, .2)),
				new Vec(2, 0)
			);
		}
		if (ch == "|") return new Lava(pos, new Vec(0, 2));
		if (ch == "v") return new Lava(pos, new Vec(0, 3), pos)
    }
    update(time, state) {
        let pos = this.pos.plus(this.speed.times(time));
        if (!state.level.touches(pos, this.size, "wall")) return new Lava(pos, this.speed, this.reset);
        if (this.reset) return new Lava(this.reset, this.speed, this.reset);
        return new Lava(this.pos, this.speed.times(-1))
    }
    collide(state) {
        return new State(state.level, state.actors, "lost")
    }
}

Lava.prototype.size = new Vec(1, 1);
class Coin {
    constructor(pos, basePos, wobble) {
        this.pos = pos;
        this.basePos = basePos;
        this.wobble = wobble
    }
    get type() {
        return "coin"
    }
    static create(pos) {
        let base = pos.plus(new Vec(.2, .1));
        return new Coin(base, base, Math.random() * Math.PI * 2)
    }
    update(time) {
        let wobble = this.wobble + time * 8;
        return new Coin(this.basePos.plus(new Vec(0, Math.sin(wobble) * .07)), this.basePos, wobble)
    }
    collide(state) {
        let actors = state.actors.filter(a => a != this);
		playerMode = "Invisibility";
		postBoostName(playerMode);
		///////
        return new State(state.level, actors, "Invisibility")
        // return new State(state.level, actors, state.status)
        // return new State(state.level, actors, actors.some(a => a.type == "coin") ? state.status : "won")
    }
}
Coin.prototype.size = new Vec(.6, .6);
const levelChars = {
    ".": "empty",
    "#": "wall",
    "+": "lava",
    "@": Player,
    "o": Coin,
    "=": Lava,
    "|": Lava,
    "v": Lava
};

function elt(name, attrs, ...children) {
    let dom = document.createElement(name);
    for (let attr of Object.keys(attrs)) dom.setAttribute(attr, attrs[attr]);
    for (let child of children) dom.appendChild(child);
    return dom
}
const scale = 70;

function drawGrid(level) {
    return elt("table", {
        class: "background",
        style: `width:${level.width*scale}px`
    }, ...level.rows.map(row => elt("tr", {
        style: `height:${scale}px`
    }, ...row.map(type => elt("td", {
        class: type
    })))))
}

function drawActors(actors) {
    return elt("div", {}, ...actors.map(actor => {
        let actorClass = `actor ${actor.type}`;

		if (actor.type === "player") {
			actorClass += ` ${actor.animation}`;
		}

		let rect = elt("div", {
			class: actorClass
		});
        rect.style.width = `${actor.size.x*scale}px`;
        rect.style.height = `${actor.size.y*scale}px`;
        rect.style.left = `${actor.pos.x*scale}px`;
        rect.style.top = `${actor.pos.y*scale}px`;
        return rect
    }))
}

class DOMDisplay {
    constructor(parent, level) {
        this.dom = elt("div", {
            class: "game"
        }, drawGrid(level));
        this.actorLayer = null;
        parent.appendChild(this.dom)
    }
    syncState(state) {
        if (this.actorLayer) this.actorLayer.remove();
        this.actorLayer = drawActors(state.actors);
        this.dom.appendChild(this.actorLayer);
        this.dom.className = `game ${state.status}`;
        let player = state.player,
            center = player.pos.plus(player.size.times(.5)).times(scale);
        // this.dom.scrollLeft = center.x - this.dom.clientWidth / 2;
        // this.dom.scrollTop = center.y - this.dom.clientHeight / 2
        if (center.x > this.dom.clientWidth * 0.15) {
            this.dom.scrollLeft = center.x - this.dom.clientWidth * 0.15;
        }
    }
    clear() {
        this.dom.remove()
    }
}

function overlap(a, b) {
    return a.pos.x + a.size.x > b.pos.x && a.pos.x < b.pos.x + b.size.x && a.pos.y + a.size.y > b.pos.y && a.pos.y < b.pos.y + b.size.y
}

function trackKeys(keys) {
    let down = Object.create(null);

    function track(event) {
        if (keys.includes(event.key)) {
            down[event.key] = event.type == "keydown";
            event.preventDefault()
        }
    }
    window.addEventListener("keydown", track);
    window.addEventListener("keyup", track);
    return down
}
const arrowKeys = trackKeys(["ArrowLeft", "ArrowRight", "ArrowUp", "ArrowDown"]);

function runAnimation(frameFunc) {
    let lastTime = null;

    function frame(time) {
        if (lastTime != null) {
            let step = Math.min(time - lastTime, 100) / 1000;
            if (frameFunc(step) === false) return
        }
        lastTime = time;
        requestAnimationFrame(frame)
    }
    requestAnimationFrame(frame)
}

function runLevel(level, Display, session) {
    let display = new Display(document.body, level),
        state = State.start(level),
        ending = 1;
    return new Promise(resolve => runAnimation(time => {
        if (session !== gameSession || openedPopup) {
            display.clear();
            resolve("stopped");
            return false;
        }
        state = state.update(time, arrowKeys);

		if (state.status === "lost") {
			stopCurrentGame();
		}

        display.syncState(state);

        if (state.status == "playing") return true;

        if (ending > 0) {
            ending -= time;
            return true
        }
        display.clear();
        resolve(state.status);
        return false
    }))
}


async function runGame(plans, Display, session) {
    for (let level = 0; level < plans.length;) {
        let result = await runLevel(new Level(plans[level]), Display, session);
		if (result === "stopped") return;
        if (result === "lost"){
			console.log("Timer stoppet efter " + (decisecond / 10) + " sekunder");
			console.log("Player dead!")
			return;
		}
        // if (result === "won") level++;
    }
	resetGame();
    console.log("Game is over!")
}
