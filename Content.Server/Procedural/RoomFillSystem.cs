// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Map.Components;

namespace Content.Server.Procedural;

public sealed class RoomFillSystem : EntitySystem
{
    [Dependency] private readonly DungeonSystem _dungeon = default!;
    [Dependency] private readonly SharedMapSystem _maps = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RoomFillComponent, MapInitEvent>(OnRoomFillMapInit);
    }

    private void OnRoomFillMapInit(EntityUid uid, RoomFillComponent component, MapInitEvent args)
    {
        // Just test things.
        if (component.Size == Vector2i.Zero)
            return;

        var xform = Transform(uid);

        if (xform.GridUid != null)
        {
            var random = new Random();
            var room = _dungeon.GetRoomPrototype(component.Size, random, component.RoomWhitelist);

            if (room != null)
            {
                var mapGrid = Comp<MapGridComponent>(xform.GridUid.Value);
                _dungeon.SpawnRoom(
                    xform.GridUid.Value,
                    mapGrid,
                    _maps.LocalToTile(xform.GridUid.Value, mapGrid, xform.Coordinates),
                    room,
                    random,
                    clearExisting: component.ClearExisting,
                    rotation: component.Rotation);
            }
            else
            {
                Log.Error($"Unable to find matching room prototype for {ToPrettyString(uid)}");
            }
        }

        // Final cleanup
        QueueDel(uid);
    }
}
