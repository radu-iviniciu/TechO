import json
from copy import deepcopy
from tabulate import tabulate
# from pprint import pprint

# 1 wall
# 2 box
# 3 storage
# 4 robot
# 5 stray

#   1
# 4 0 3
#   2


def init_matrix(rows, cols, count, objects):
    # first index col, second index row
    matrix = [[0 for i in range(cols)] for j in range(rows)]
    index = 0
    while index < 3*count:
        matrix[objects[index + 2]][objects[index + 1]] = objects[index]
        index += 3
    return matrix


def prep_bfs(matrix, init):
    for i, row in enumerate(matrix):
        for j, elem in enumerate(row):
            if elem == 1 or elem == 2:
                matrix[i][j] = -1
            else:
                matrix[i][j] = 100
    matrix[init[0]][init[1]] = 0
    return matrix


def bfs(matrix, init):
    visit = [init]
    while visit:
        # print(tabulate(matrix))
        current = visit.pop(0)
        i, j = current[0], current[1]
        if matrix[i-1][j] > matrix[i][j]:
            matrix[i-1][j] = matrix[i][j] + 1
            visit.append((i-1, j))
        if matrix[i+1][j] > matrix[i][j]:
            matrix[i+1][j] = matrix[i][j] + 1
            visit.append((i+1, j))
        if matrix[i][j-1] > matrix[i][j]:
            matrix[i][j-1] = matrix[i][j] + 1
            visit.append((i, j-1))
        if matrix[i][j+1] > matrix[i][j]:
            matrix[i][j+1] = matrix[i][j] + 1
            visit.append((i, j+1))


def bfs_targeted(matrix, init, dest):
    visit = [init]
    while matrix[dest[0]][dest[1]] >= 100:
        # print(tabulate(matrix))
        try:
            current = visit.pop(0)
        except IndexError:
            return False
        i, j = current[0], current[1]
        if matrix[i-1][j] > matrix[i][j]:
            matrix[i-1][j] = matrix[i][j] + 1
            visit.append((i-1, j))
        if matrix[i+1][j] > matrix[i][j]:
            matrix[i+1][j] = matrix[i][j] + 1
            visit.append((i+1, j))
        if matrix[i][j-1] > matrix[i][j]:
            matrix[i][j-1] = matrix[i][j] + 1
            visit.append((i, j-1))
        if matrix[i][j+1] > matrix[i][j]:
            matrix[i][j+1] = matrix[i][j] + 1
            visit.append((i, j+1))
    return True


def opposite(dir):
    if dir == 1:
        return 2
    elif dir == 2:
        return 1
    elif dir == 3:
        return 4
    elif dir == 4:
        return 3
    else:
        return -1


def apply_corrections(moves):
    i = 0
    while i < len(moves)-1:
        if (moves[i] in [1, 2] and moves[i+1] in [3, 4]) or \
          (moves[i] in [3, 4] and moves[i+1] in [1, 2]):
            moves.insert(i+1, opposite(moves[i+1]))
            moves.insert(i+2, moves[i])
            i += 3
        else:
            i += 1


def find_moves(matrix, box, target, do_corrections):
    current = target
    moves = []
    while current != box:
        try:
            i, j = current[0], current[1]
            if matrix[i-1][j] < matrix[i][j] and matrix[i-1][j] >= 0:
                moves.append(2)
                current = (i-1, j)
            elif matrix[i+1][j] < matrix[i][j] and matrix[i+1][j] >= 0:
                moves.append(1)
                current = (i+1, j)
            elif matrix[i][j-1] < matrix[i][j] and matrix[i][j-1] >= 0:
                moves.append(3)
                current = (i, j-1)
            elif matrix[i][j+1] < matrix[i][j] and matrix[i][j+1] >= 0:
                moves.append(4)
                current = (i, j+1)
            # print(current)
        except IndexError:
            pass
    if do_corrections:
        moves.reverse()
        apply_corrections(moves)
    return moves


def get_pos_related_to_box(last_move, box):
    if last_move == 1:
        pos = (box[0] + 1, box[1])
    elif last_move == 2:
        pos = (box[0] - 1, box[1])
    elif last_move == 3:
        pos = (box[0], box[1] - 1)
    elif last_move == 4:
        pos = (box[0], box[1] + 1)
    else:
        raise AttributeError
    return pos


def track_moves(matrix, moves, box):
    robot = get_pos_related_to_box(moves[0], box)
    starting_position = robot
    # print("here for: ", robot)
    if matrix[robot[0]][robot[1]] == -1:
        return (False, robot, starting_position)

    for mv in moves:
        prev_robot = deepcopy(robot)
        if mv == 1:
            robot = (robot[0] - 1, robot[1])
        elif mv == 2:
            robot = (robot[0] + 1, robot[1])
        elif mv == 3:
            robot = (robot[0], robot[1] + 1)
        elif mv == 4:
            robot = (robot[0], robot[1] - 1)
        if matrix[robot[0]][robot[1]] == -1:
            return (False, prev_robot, starting_position)
    return (True, prev_robot, starting_position)


def update_gold(gold_matrix, last_move, new_box, old_box, old_robot):
    new_robot = get_pos_related_to_box(last_move, new_box)
    gold_matrix[old_box[0]][old_box[1]] = 0
    gold_matrix[old_robot[0]][old_robot[1]] = 0
    gold_matrix[new_box[0]][new_box[1]] = 1
    gold_matrix[new_robot[0]][new_robot[1]] = 4


def compute_moves(input_rows, input_cols, input_objects_count, input_objects):
    gold_matrix = init_matrix(input_rows, input_cols,
                              input_objects_count, input_objects)
    # pprint_map(gold_matrix)
    robot = (objects[2], objects[1])
    boxes = [(i, j)
             for i in range(rows)
             for j in range(cols)
             if gold_matrix[i][j] == 2]
    storages = [(i, j)
                for i in range(rows)
                for j in range(cols)
                if gold_matrix[i][j] == 3]

    for box in boxes:
        valid_moves = False
        local_gold = deepcopy(gold_matrix)
        max_iterations = 0
        while not valid_moves and max_iterations < 3:
            matrix = prep_bfs(deepcopy(local_gold), box)
            # print(tabulate(matrix))
            bfs(matrix, box)
            min_storage = 300
            target = (0, 0)
            for storage in storages:
                if min_storage > matrix[storage[0]][storage[1]]:
                    min_storage = matrix[storage[0]][storage[1]]
                    target = storage
            # print(tabulate(matrix))
            print(str(box) + " -> " + str(target))
            if (min_storage == 100):
                max_iterations += 1
                continue
            moves = find_moves(matrix, box, target, True)
            # moves are reversed here
            # print(moves)
            (valid_moves, wall, starting_pos) = track_moves(matrix, moves,
                                                            deepcopy(box))
            moves.reverse()
            if not valid_moves:
                # print("new wall: ", wall)
                local_gold[wall[0]][wall[1]] = 1
            max_iterations += 1
        if not valid_moves:
            print("could not find moves")
        else:
            # print("valid moves: ", moves)
            # move robot from current position to near the box
            matrix = prep_bfs(deepcopy(gold_matrix), robot)
            if bfs_targeted(matrix, robot, starting_pos):
                print(matrix[starting_pos[0]][starting_pos[1]])
                robot_moves = find_moves(matrix, robot, starting_pos, False)
                moves += robot_moves
                print("full moves: ", moves)
                update_gold(gold_matrix, moves[0], target, box, robot)
                robot = get_pos_related_to_box(moves[0], target)
                print(tabulate(gold_matrix))
                storages.remove(target)
            else:
                print("game over")


def pprint_map(curr_map):
    for row in curr_map:
        print("\n", end="")
        for elem in row:
            print(elem, end="")
    print("\n")


def getXobj(concept):
    return concept["position"]["x"]


def getYobj(concept):
    return concept["position"]["y"]


def getX(game, concept):
    return game[concept]["position"]["x"]


def getY(game, concept):
    return game[concept]["position"]["y"]

with open('05_intoTheDarkness.txt') as data_file:
    data = json.load(data_file)

index = 1
for game in data:
    print("HARTA " + str(index))
    objects = []
    index += 1
    rows = game["rows"]
    cols = game["cols"]
    curr_map = [["." for j in range(cols)] for i in range(rows)]

    # set player
    curr_map[getY(game, "player")][getX(game, "player")] = "@"
    objects.extend((4, getX(game, "player"), getY(game, "player")))
    obj_count = 1
    try:
        # set adversary
        curr_map[getY(game, "adversary")][getX(game, "adversary")] = "#"
        objects.extend((5, getX(game, "adversary"), getY(game, "adversary")))
        obj_count += 1
    except KeyError as err:
        print("No adversary")

    # set objects
    for obj in game["objects"]:
        obj_count += 1
        ident = "W"
        if obj["type"] == "Box":
            ident = "B"
            objects.extend((2, getXobj(obj), getYobj(obj)))
        elif obj["type"] == "Storage":
            ident = "S"
            objects.extend((3, getXobj(obj), getYobj(obj)))
        else:
            ident = "W"  # redundant, but more visible
            objects.extend((1, getXobj(obj), getYobj(obj)))
        curr_map[getYobj(obj)][getXobj(obj)] = ident

    pprint_map(curr_map)
    moves = compute_moves(rows, cols, obj_count, objects)