from select import select
from tkinter.constants import CASCADE

import pygame, sys

pygame.init()
font=pygame.font.SysFont(CASCADE,80)
#Making the game window
screen = pygame.display.set_mode((750,750))
#Window text and sudoku grid image
pygame.display.set_caption("SUDOKU GAME")

selected_cell=None

grid=[
    [5, 3, 0, 0, 7, 0, 0, 0, 0],
    [6, 0, 0, 1, 9, 5, 0, 0, 0],
    [0, 9, 8, 0, 0, 0, 0, 6, 0],
    [8, 0, 0, 0, 6, 0, 0, 0, 3],
    [4, 0, 0, 8, 0, 3, 0, 0, 1],
    [7, 0, 0, 0, 2, 0, 0, 0, 6],
    [0, 6, 0, 0, 0, 0, 2, 8, 0],
    [0, 0, 0, 4, 1, 9, 0, 0, 5],
    [0, 0, 0, 0, 8, 0, 0, 7, 9]
]

def draw_background():
    screen.fill((173,206,235)) #background colour
    pygame.draw.rect(screen, (255,255,255), pygame.Rect(15,15,720,725),10) #drawing our outer rectangle for the grid
    i=1
    while (i*80)<720:
        line_width=5 if i%3>0 else 10 #making the subgrids more visible
        pygame.draw.line(screen, (255,255,255), (i*80+15,15),(i*80+15,735),line_width) #will give us 9 equal width vertical lines
        pygame.draw.line(screen, (255,255,255), (15,(i * 80) + 15), (735, (i * 80) + 15), line_width) #Horizontal lines
        i+=1

def draw_numbers():
    row=0
    offset=42
    while row<9:
        column = 0
        while column<9:
            out=grid[row][column]
            if out!=0:
              nums=font.render(str(out),True,(100,149,237))
              screen.blit(nums,pygame.Vector2((column*80)+offset,(row*80)+offset-6))
            column+=1
        row+=1

def draw_highlight():
    if selected_cell:
        x,y=selected_cell
        pygame.draw.rect(screen,(255,223,186),pygame.Rect(x*80+15,y*80+15,80,80),5)
def game_loop():
    global selected_cell
    for event in pygame.event.get():
        if event.type==pygame.QUIT:sys.exit() #if we press the exit button it quits the game
        elif event.type== pygame.MOUSEBUTTONDOWN:
            x,y=pygame.mouse.get_pos()
            selected_cell=((x-15)//80,(y-15)//80)
        elif event.type==pygame.KEYDOWN:
            if selected_cell and event.unicode.isdigit() and event.unicode !='0':
                x,y=selected_cell
                grid[x,y]=int(event.unicode)

    draw_background()
    draw_numbers()
    draw_highlight()
    pygame.display.flip()

while True:
    game_loop()







