export async function getDevices(params) {
    const response = await fetch(`/LockerDevice/GetData/?${new URLSearchParams(params)}`);
    return response.json();
}

export async function getRoom(id) {
    const response = await fetch(`/LockerRoom/GetLockerRoom/?id=${id}`);
    return response.json();
}

export async function saveRoom(data) {
    await fetch(`/LockerRoom/SaveLockerRoom/`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(data)
    });
}

export async function deleteRoom(id) {
    await fetch(`/LockerRoom/DeleteLockerRoom/?id=${id}`, {method: 'DELETE'});
}
