   M S B A M L   `   `   `  ���� F  BBattleships, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:BattleshipsBattleships      Battleships.LobbyView   R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    >d2http://schemas.microsoft.com/expression/blend/2008    )localclr-namespace:Battleships    $	��230��$	��500��
�������.���   � RowDefinitions  �� $
��Auto ��� $
��Auto ��� $
��Auto �$��129.732���� $��0 ��� $
��Auto ���  � ColumnDefinitions �� $
��Auto ��� $
��Auto ��� .+��� -    ��Name$ labelOnlinePlayers�� �� ContentStringFormat$ {0} Players online��$��0��$��83.174,4,4,0q�$��2���� -   $ labelPlayersAvailable��$��Players available��$��
31.756,3.5q�$��1���� -   $ labelSentInvites��$��Sent invites��$��	44.71,3.5q�$��1��$��1���� -   $ labelIncomingInvites��$��Incoming invites��$��41.636,3.5,41.635,3.5q�$��1��$��2���� -    ��Name$ listBoxPlayersAvailable��$	��3.5q�$��2��$ �� IsSynchronizedWithCurrentItem$
 TrueY�$��0��$	��140��$	��120��$��Center=�$��Center=����� .R��� ���� *
Nickname+�� -    ��Name$ buttonSendInvite��$��Send invite��$��Center=�$	��3.5q�$��3��$��Center=�$��1q�$��2���� -   $ listBoxSentPlayerInvites��$	��3.5q�$��2��$
 TrueY�$��1��$	��140��$	��120��$��Center=�$��Center=����� .R��� ���� *
Nickname+�� -   $ labelLocalPlayerName��$ Welcome, {0}��$�� ��$	��3.5q�$��Center=�$
��Bold"�$��Center=�$��2���� -	   $" listBoxReceivedPlayerInvites��$	��3.5q�$��2��$
 TrueY�$��3��$	��140��$	��120��$��Center=�$��Center=����� .R��� ���� *
Nickname+�� -
   $ buttonRevokeInvite��$��Revoke invite��$��Center=�$	��3.5q�$��3��$��1��$��Center=�$��1q�$��2���� -   $ buttonAcceptInvite��$
��Play��$��Center=�$	��3.5q�$��3��$��2��$��Center=�$��1q�$��2��