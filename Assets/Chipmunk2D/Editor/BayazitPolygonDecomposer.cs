using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// From phed rev 36
/// Convex decomposition algorithm created by Mark Bayazit (http://mnbayazit.com/)
/// For more information about this algorithm, see http://mnbayazit.com/406/bayazit
/// This class is took from the "FarseerUnity" physics engine, which uses Mark Bayazit's decomposition algorithm.
/// I also have to make it work with self-intersecting polygons, so I'll use another different algorithm to decompose a 
/// self-intersecting polygon into several simple polygons, and then I would decompose each of them into convex polygons.
/// </summary>
public static class BayazitPolygonDecomposer {

	/// <summary>
	/// Decompose the polygon into several smaller non-concave polygon.
	/// If the polygon is already convex, it will return the original polygon, unless it is over Settings.MaxPolygonVertices.
	/// Precondition: Counter Clockwise polygon.
	/// </summary>
	/// <param name="vertices"></param>
	/// <returns></returns>
	public static List<Vector2[]> ConvexPartition(Vector3[] vertices) {
		List<Vector2> vec2List = new List<Vector2>(vertices.Length);
		foreach(Vector3 point in vertices) {
			vec2List.Add((Vector2)point);
		}
		return ConvexPartition(vec2List);
	}

    /// <summary>
    /// Decompose the polygon into several smaller non-concave polygon.
    /// If the polygon is already convex, it will return the original polygon, unless it is over Settings.MaxPolygonVertices.
    /// Precondition: Counter Clockwise polygon.
    /// </summary>
    /// <param name="vertices"></param>
    /// <returns></returns>
    public static List<Vector2[]> ConvexPartition(List<Vector2> vertices) {
        //We force it to CCW as it is a precondition in this algorithm.
        ForceCounterClockWise(vertices);

        List <Vector2[]> resultList = new List <Vector2[]> ();
        float d, lowerDist, upperDist;
        Vector2 p;
        Vector2 lowerInt = new Vector2();
        Vector2 upperInt = new Vector2(); // intersection points
        int lowerIndex = 0, upperIndex = 0;
        List < Vector2 > lowerPoly, upperPoly;

        for (int i = 0; i < vertices.Count; ++i) {
            if (!Reflex(i, vertices))
                continue;

            lowerDist = upperDist = float.MaxValue; // std::numeric_limits<qreal>::max();
            for (int j = 0; j < vertices.Count; ++j) {
                // if line intersects with an edge
                if (Left(At(i - 1, vertices), At(i, vertices), At(j, vertices)) && RightOn(At(i - 1, vertices), At(i, vertices), At(j - 1, vertices))) {
                    // find the point of intersection
                    p = LineIntersect(At(i - 1, vertices), At(i, vertices), At(j, vertices),
                    At(j - 1, vertices));
                    if (Right(At(i + 1, vertices), At(i, vertices), p)) {
                        // make sure it's inside the poly
                        d = SquareDist(At(i, vertices), p);
                        if (d < lowerDist) {
                            // keep only the closest intersection
                            lowerDist = d;
                            lowerInt = p;
                            lowerIndex = j;
                        }
                    }
                }

                if (Left(At(i + 1, vertices), At(i, vertices), At(j + 1, vertices)) && RightOn(At(i + 1, vertices), At(i, vertices), At(j, vertices))) {
                    p = LineIntersect(At(i + 1, vertices), At(i, vertices), At(j, vertices),
                    At(j + 1, vertices));
                    if (Left(At(i - 1, vertices), At(i, vertices), p)) {
                        d = SquareDist(At(i, vertices), p);
                        if (d < upperDist) {
                            upperDist = d;
                            upperIndex = j;
                            upperInt = p;
                        }
                    }
                }
            }

            // if there are no vertices to connect to, choose a point in the middle
            if (lowerIndex == (upperIndex + 1) % vertices.Count) {
                Vector2 sp = ((lowerInt + upperInt) / 2);
                lowerPoly = Copy(i, upperIndex, vertices);
                lowerPoly.Add(sp);
                upperPoly = Copy(lowerIndex, i, vertices);
                upperPoly.Add(sp);
            } else {
                double highestScore = 0, bestIndex = lowerIndex;
                while (upperIndex < lowerIndex) upperIndex += vertices.Count;
                for (int k = lowerIndex; k <= upperIndex; ++k) {
                    if (CanSee(i, k, vertices)) {
                        double score = 1 / (SquareDist(At(i, vertices), At(k, vertices)) + 1);
                        if (Reflex(k, vertices)) {
                            if (RightOn(At(k - 1, vertices), At(k, vertices), At(i, vertices)) && LeftOn(At(k + 1, vertices), At(k, vertices), At(i, vertices))) {
                                score += 3;
                            } else {
                                score += 2;
                            }
                        } else {
                            score += 1;
                        }
                        if (score > highestScore) {
                            bestIndex = k;
                            highestScore = score;
                        }
                    }
                }

                lowerPoly = Copy(i, (int) bestIndex, vertices);
                upperPoly = Copy((int) bestIndex, i, vertices);
            }

            resultList.AddRange(ConvexPartition(lowerPoly));
            resultList.AddRange(ConvexPartition(upperPoly));
            return resultList;
        }

        // polygon is already convex
        resultList.Add(vertices.ToArray());

        //The polygons are not guaranteed to be without collinear points. We remove them to be sure.
        //for (int i = 0; i < resultList.Count; i++) {
        //    resultList[i] = SimplifyTools.CollinearSimplify(resultList[i], 0);
        //}

        //Remove empty vertice collections
        for (int i = resultList.Count - 1; i >= 0; i--) {
            if (resultList[i].Length == 0) resultList.RemoveAt(i);
        }

        return resultList;
    }

    private static Vector2 At(int i, List < Vector2 > vertices) {
        int s = vertices.Count;
        int index = i < 0 ? s - (-i % s) : i % s;
        return vertices[index];
    }

    private static List < Vector2 > Copy(int i, int j, List < Vector2 > vertices) {
        List < Vector2 > p = new List < Vector2 > ();
        while (j < i)
            j += vertices.Count;
        //p.reserve(j - i + 1);
        for (; i <= j; ++i) {
            p.Add(At(i, vertices));
        }
        return p;
    }

    private static bool CanSee(int i, int j, List < Vector2 > vertices) {
        if (Reflex(i, vertices)) {
            if (LeftOn(At(i, vertices), At(i - 1, vertices), At(j, vertices)) && RightOn(At(i, vertices), At(i + 1, vertices), At(j, vertices))) return false;
        } else {
            if (RightOn(At(i, vertices), At(i + 1, vertices), At(j, vertices)) || LeftOn(At(i, vertices), At(i - 1, vertices), At(j, vertices))) return false;
        }
        if (Reflex(j, vertices)) {
            if (LeftOn(At(j, vertices), At(j - 1, vertices), At(i, vertices)) && RightOn(At(j, vertices), At(j + 1, vertices), At(i, vertices))) return false;
        } else {
            if (RightOn(At(j, vertices), At(j + 1, vertices), At(i, vertices)) || LeftOn(At(j, vertices), At(j - 1, vertices), At(i, vertices))) return false;
        }
        for (int k = 0; k < vertices.Count; ++k) {
            if ((k + 1) % vertices.Count == i || k == i || (k + 1) % vertices.Count == j || k == j) {
                continue; // ignore incident edges
            }
            Vector2 intersectionPoint;
            if (LineIntersect2(At(i, vertices), At(j, vertices), At(k, vertices), At(k + 1, vertices), out intersectionPoint)) {
                return false;
            }
        }
        return true;
    }

    // precondition: ccw
    private static bool Reflex(int i, List < Vector2 > vertices) {
        return Right(i, vertices);
    }

    private static bool Right(int i, List < Vector2 > vertices) {
        return Right(At(i - 1, vertices), At(i, vertices), At(i + 1, vertices));
    }

    private static bool Left(Vector2 a, Vector2 b, Vector2 c) {
        return Area(ref a, ref b, ref c) > 0;
    }

    private static bool LeftOn(Vector2 a, Vector2 b, Vector2 c) {
        return Area(ref a, ref b, ref c) >= 0;
    }

    private static bool Right(Vector2 a, Vector2 b, Vector2 c) {
        return Area(ref a, ref b, ref c) < 0;
    }

    private static bool RightOn(Vector2 a, Vector2 b, Vector2 c) {
        return Area(ref a, ref b, ref c) <= 0;
    }

    private static float SquareDist(Vector2 a, Vector2 b) {
        float dx = b.x - a.x;
        float dy = b.y - a.y;
        return dx * dx + dy * dy;
    }

    // forces counter clock wise order.
    private static void ForceCounterClockWise(List < Vector2 > vertices) {
        if (!IsCounterClockWise(vertices)) {
            vertices.Reverse();
        }
    }

    private static bool IsCounterClockWise(List < Vector2 > vertices) {
        //We just return true for lines
        if (vertices.Count < 3) return true;

        return (GetSignedArea(vertices) > 0.0f);
    }

    // gets the signed area.
    private static float GetSignedArea(List < Vector2 > vertices) {
        int i;
        float area = 0;

        for (i = 0; i < vertices.Count; i++) {
            int j = (i + 1) % vertices.Count;
            area += vertices[i].x * vertices[j].y;
            area -= vertices[i].y * vertices[j].x;
        }
        area /= 2.0f;
        return area;
    }

    // From Mark Bayazit's convex decomposition algorithm
    private static Vector2 LineIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2) {
        Vector2 i = Vector2.zero;
        float a1 = p2.y - p1.y;
        float b1 = p1.x - p2.x;
        float c1 = a1 * p1.x + b1 * p1.y;
        float a2 = q2.y - q1.y;
        float b2 = q1.x - q2.x;
        float c2 = a2 * q1.x + b2 * q1.y;
        float det = a1 * b2 - a2 * b1;

        if (!FloatEquals(det, 0)) {
            // lines are not parallel
            i.x = (b2 * c1 - b1 * c2) / det;
            i.y = (a1 * c2 - a2 * c1) / det;
        }
        return i;
    }

    // From Eric Jordan's convex decomposition library, it checks if the lines a0->a1 and b0->b1 cross.
    // If they do, intersectionPoint will be filled with the point of crossing. Grazing lines should not return true.
    private static bool LineIntersect2(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1, out Vector2 intersectionPoint) {
        intersectionPoint = Vector2.zero;

        if (a0 == b0 || a0 == b1 || a1 == b0 || a1 == b1) return false;

        float x1 = a0.x;
        float y1 = a0.y;
        float x2 = a1.x;
        float y2 = a1.y;
        float x3 = b0.x;
        float y3 = b0.y;
        float x4 = b1.x;
        float y4 = b1.y;

        //AABB early exit
        if (Mathf.Max(x1, x2) < Mathf.Min(x3, x4) || Mathf.Max(x3, x4) < Mathf.Min(x1, x2)) return false;

        if (Mathf.Max(y1, y2) < Mathf.Min(y3, y4) || Mathf.Max(y3, y4) < Mathf.Min(y1, y2)) return false;

        float ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3));
        float ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3));
        float denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
        if (Mathf.Abs(denom) < Mathf.Epsilon) {
            //Lines are too close to parallel to call
            return false;
        }
        ua /= denom;
        ub /= denom;

        if ((0 < ua) && (ua < 1) && (0 < ub) && (ub < 1)) {
            intersectionPoint.x = (x1 + ua * (x2 - x1));
            intersectionPoint.y = (y1 + ua * (y2 - y1));
            return true;
        }

        return false;
    }

    private static bool FloatEquals(float value1, float value2) {
        return Mathf.Abs(value1 - value2) <= Mathf.Epsilon;
    }

    // Returns a positive number if c is to the left of the line going from a to b. Positive number if point is left,
    // negative if point is right,and 0 if points are collinear.</returns>
    private static float Area(Vector2 a, Vector2 b, Vector2 c) {
        return Area(ref a, ref b, ref c);
    }

    // returns a positive number if c is to the left of the line going from a to b. Positive number if point is left, negative if point is right, and 0 if points are collinear.</returns>
    private static float Area(ref Vector2 a, ref Vector2 b, ref Vector2 c) {
        return a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y);
    }

    // removes all collinear points on the polygon.
    private static List < Vector2 > CollinearSimplify(List < Vector2 > vertices, float collinearityTolerance) {
        //We can't simplify polygons under 3 vertices
        if (vertices.Count < 3) return vertices;

        List < Vector2 > simplified = new List < Vector2 > ();

        for (int i = 0; i < vertices.Count; i++) {
            int prevId = PreviousIndex(vertices, i);
            int nextId = NextIndex(vertices, i);

            Vector2 prev = vertices[prevId];
            Vector2 current = vertices[i];
            Vector2 next = vertices[nextId];

            //If they collinear, continue
            bool isCollinear = Collinear(ref prev, ref current, ref next, collinearityTolerance);
            if (isCollinear)
                continue;

            simplified.Add(current);
        }

        return simplified;
    }

    // gets the previous index.
    private static int PreviousIndex(List < Vector2 > vertices, int index) {
        if (index == 0) {
            return vertices.Count - 1;
        }
        return index - 1;
    }

    // nexts the index.
    private static int NextIndex(List < Vector2 > vertices, int index) {
        if (index == vertices.Count - 1)
            return 0;
        return index + 1;
    }

    private static bool Collinear(ref Vector2 a, ref Vector2 b, ref Vector2 c, float tolerance) {
        return FloatInRange(Area(ref a, ref b, ref c), -tolerance, tolerance);
    }

    // checks if a floating point Value is within a specified range of values (inclusive).
    private static bool FloatInRange(float value, float min, float max) {
        return (value >= min && value <= max);
    }

}