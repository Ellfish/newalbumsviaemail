export default function objectEmpty(obj) {
    for (var x in obj) { return false; }
    return true;
}