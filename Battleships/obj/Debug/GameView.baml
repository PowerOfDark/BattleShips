   M S B A M L   `   `   `  ����F  BBattleships, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:BattleshipsBattleships  5          Battleships.GameView   5      R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    5      >d2http://schemas.microsoft.com/expression/blend/2008    5      )localclr-namespace:Battleships    5      $	��500��5      $	��350��6   
�������.��5	      �   � RowDefinitions  5
   
   �� 5      $��60 �6   �� 5      $
��Auto �6   �� 5      .+�5   
   � $��1��6   $��0��6    � ColumnDefinitions 5      �� 5      �� 5      .+�5         Battleships.SeaBoard  -   5          Name$ myBoard��5   -   $��0��6   $	��240��6>   $	��3.5q�6J   $
��Left=�6W   $	��240��6r     5      -   5        $ 
enemyBoard��5   I   $��1��6   $��Right=�6-   $	��240��6]   $��3.5,3.5,10,3.5q�6i   $	��240��6�   5      �� 5   
   -   5         ��Name$ labelMyNickname��5      $��WWWWWWWWWWWWWWWW��6)   $��3.5,10,3.5,3.5q�6D   $
��Left=�6~   $��5q�6�   $��Center=�6�   �� 5   
   -   5        $ labelEnemyNickname��5      $��WWWWWWWWWWWWWWWW��6,   $��Right=�6G   $��3.5,10,3.5,3.5q�6c   $��Center=�6�   $��5q�6�   �� 5   
   -   5         ��Name$ buttonGameResult��5      $��You have XXXX��6+   $��Center=�6C   $	��3.5q�6`   $��Center=�6m   $��2��6�    J� Opacity$ 0Q�6�   {�.  6�   5      5      