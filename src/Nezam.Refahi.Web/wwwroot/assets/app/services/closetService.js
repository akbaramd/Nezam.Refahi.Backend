export async function getDressRooms(params) {
    const response = await fetch(`/DressRoom/GetData?${new URLSearchParams(params)}`);
    return response.json();
}

export async function getCloset(id) {
    const response = await fetch(`/Closet/GetCloset?id=${id}`);
    return response.json();
}

export async function saveCloset(data) {
   return  await fetch(`/Closet/SaveCloset`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(data)
    });
}

export async function deleteCloset(id) {
    await fetch(`/Closet/DeleteCloset?id=${id}`, {method: 'DELETE'});
}