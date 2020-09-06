let rotateSpeed = 0.01;
let ellipsoids = [];
let triangles = [];
let rot = 1;

window.setp5 = () => {
    new p5(sketch, window.document.getElementById('container'));
    return true;
};

window.speedUp = () => {
    rotateSpeed *= 2;
};

window.fix = () => {
    rot *= -1;
}

window.addEllipsoid = (pos, one, two, three) => {

    let result = {};
    result.position = pos;
    result.rotation1 = {};
    result.rotation2 = {};
    result.axes = {};
    result.axesLengths = {};

    const a = new p5.Vector(one.x, one.y, one.z);
    const b = new p5.Vector(two.x, two.y, two.z);
    const c = new p5.Vector(three.x, three.y, three.z);

    result.axes.a = a;
    result.axes.b = b;
    result.axes.c = c;

    const aLength = a.mag();
    const bLength = b.mag();
    const cLength = c.mag();

    result.axesLengths.x = aLength;
    result.axesLengths.y = bLength;
    result.axesLengths.z = cLength;

    const x = new p5.Vector(1, 0, 0);
    const y = new p5.Vector(0, 1, 0);

    const aDotX = a.dot(x);
    const sigma = aDotX / aLength;

    const aCrossX = p5.Vector.cross(a, x);
    if (aCrossX.mag() != 0) {
        const unitACrossX = p5.Vector.div(aCrossX, aCrossX.mag());

        const axis1 = aCrossX;
        const theta1 = -Math.acos(sigma);

        const bSharp = p5.Vector.mult(b, sigma)
            .add(p5.Vector.cross(unitACrossX, b).mult(Math.sqrt(1 - sigma * sigma)))
            .add(p5.Vector.mult(unitACrossX, unitACrossX.dot(b) * (1 - sigma)));

        result.bSharp = bSharp;

        const axis2 = p5.Vector.cross(bSharp, y);
        const theta2 = -Math.acos(bSharp.dot(y) / bSharp.mag());

        if (axis1.mag() == 0) {
            result.rotation1.axis = x;
            result.rotation1.angle = 0;
        } else {
            result.rotation1.axis = axis1;
            result.rotation1.angle = theta1;
        }
        if (axis2.mag() == 0) {
            result.rotation2.axis = x;
            result.rotation2.angle = 0;
        } else {
            result.rotation2.axis = axis2;
            result.rotation2.angle = theta2;
        }

    } else {
        const theta = Math.acos(b.dot(y) / b.mag());

        result.rotation1.axis = x;
        result.rotation1.angle = theta;

        result.rotation2.axis = x;
        result.rotation2.angle = 0;
    }

    ellipsoids.push(result);
}


window.printPolyhedron = (trigs) => {
    triangles = trigs;
};

window.clearEllipsoids = () => {
    ellipsoids = [];
};

let sketch = function(sketch) {

    sketch.setup = function() {

        let renderer = sketch.createCanvas(window.innerWidth, window.innerHeight / 2, sketch.WEBGL);
        // renderer.drawingContext.disable(renderer.drawingContext.DEPTH_TEST);
        sketch.frameRate(30);

        sketch.createEasyCam();
        eyeZ = ((sketch.height/2.0) / sketch.tan(sketch.PI * 60.0/360.0));
        sketch.perspective(sketch.PI/3.0, sketch.width/sketch.height, eyeZ / 10.0, eyeZ * 100.0);

        // To ignore right clicks and scrolling
        document.getElementById('container').oncontextmenu = () => false;
        document.getElementById('container').onmousewheel = () => false;
    }

    sketch.draw = function() {

        sketch.background(200);

        // sketch.axes();

        sketch.fill(23, 90, 273, 200);
        sketch.stroke(255, 255, 255);
        sketch.polygon(triangles);

        sketch.fill(237, 34, 93, 70);
        sketch.stroke(237, 34, 93);
        sketch.noFill();

        for (let i = 0; i < ellipsoids.length; i++) {
            let ellipsoid = ellipsoids[i];

            if (i > 0) {
                let center = ellipsoid.position;
                let lastCenter = ellipsoids[i - 1].position;
                sketch.push();
                if (i + 1 < ellipsoids.length) {
                    sketch.stroke(0, 0, 0);
                    sketch.strokeWeight(7);
                } else {
                    sketch.stroke(255, 0, 0);
                    sketch.strokeWeight(8);
                }
                sketch.line(center.x, center.y, center.z, lastCenter.x, lastCenter.y, lastCenter.z);
                sketch.pop();
            }

            if (i + 2 >= ellipsoids.length) {
                sketch.push();
                sketch.nextColor(i + 100);

                sketch.strokeWeight(2);
                sketch.nextColor(i);

                sketch.translate(ellipsoid.position.x, ellipsoid.position.y, ellipsoid.position.z);
                sketch.rotate(ellipsoid.rotation1.angle, ellipsoid.rotation1.axis);
                sketch.rotate(ellipsoid.rotation2.angle * rot, ellipsoid.rotation2.axis);
                sketch.ellipsoid(ellipsoid.axesLengths.x, ellipsoid.axesLengths.y, ellipsoid.axesLengths.z);

                sketch.pop();
            }
        }
    }

    sketch.polygon = function(triangles) {
        for (const triangle of triangles) {
            sketch.beginShape(sketch.TESS);
            sketch.vertex(triangle.a.x, triangle.a.y, triangle.a.z);
            sketch.vertex(triangle.b.x, triangle.b.y, triangle.b.z);
            sketch.vertex(triangle.c.x, triangle.c.y, triangle.c.z);
            sketch.endShape(sketch.CLOSE);
        }
    }

    sketch.nextColor = function(i) {
        i += 1;
        sketch.stroke(186 * i % 256, 17 * i % 256, 33 * i % 256);
    }

    sketch.axes = function() {
        sketch.push();
        sketch.stroke(255, 0, 0);
        sketch.beginShape(sketch.Lines);
        sketch.vertex(0, 0, 0);
        sketch.vertex(100, 0, 0);
        sketch.endShape();

        sketch.stroke(0, 255, 0);
        sketch.beginShape(sketch.Lines);
        sketch.vertex(0, 0, 0);
        sketch.vertex(0, 100, 0);
        sketch.endShape();

        sketch.stroke(0, 0, 255);
        sketch.beginShape(sketch.Lines);
        sketch.vertex(0, 0, 0);
        sketch.vertex(0, 0, 100);
        sketch.endShape();
        sketch.pop();
    }
};
