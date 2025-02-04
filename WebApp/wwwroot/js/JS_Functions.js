async function encryptRsa(publicKey, data) {
    const encoder = new TextEncoder();
    const encodedData = encoder.encode(data);

    const publicKeyImported = await window.crypto.subtle.importKey(
        "spki",
        base64ToArrayBuffer(publicKey),
        { name: "RSA-OAEP", hash: "SHA-256" },
        false,
        ["encrypt"]
    );

    const encryptedData = await window.crypto.subtle.encrypt(
        { name: "RSA-OAEP" },
        publicKeyImported,
        encodedData
    );

    return arrayBufferToBase64(encryptedData);
}

async function encryptAes(password, data, iterations = 600000) {
    const encoder = new TextEncoder();
    const encodedData = encoder.encode(data);

    const salt = window.crypto.getRandomValues(new Uint8Array(16));
    const iv = window.crypto.getRandomValues(new Uint8Array(16));

    const keyMaterial = await window.crypto.subtle.importKey(
        "raw",
        encoder.encode(password),
        { name: "PBKDF2" },
        false,
        ["deriveKey"]
    );

    const key = await window.crypto.subtle.deriveKey(
        {
            name: "PBKDF2",
            salt: salt,
            iterations: iterations,
            hash: "SHA-512",
        },
        keyMaterial,
        { name: "AES-CBC", length: 256 },
        true,
        ["encrypt"]
    );

    const encryptedData = await window.crypto.subtle.encrypt(
        { name: "AES-CBC", iv: iv },
        key,
        encodedData
    );

    const hmacKey = await window.crypto.subtle.importKey(
        "raw",
        key,
        { name: "HMAC", hash: "SHA-256" },
        false,
        ["sign"]
    );

    const hmac = await window.crypto.subtle.sign(
        "HMAC",
        hmacKey,
        encryptedData
    );

    const combined = new Uint8Array(salt.length + iv.length + hmac.length + encryptedData.byteLength);
    combined.set(salt, 0);
    combined.set(iv, salt.length);
    combined.set(new Uint8Array(hmac), salt.length + iv.length);
    combined.set(new Uint8Array(encryptedData), salt.length + iv.length + hmac.length);

    return arrayBufferToBase64(combined.buffer);
}

function base64ToArrayBuffer(base64) {
    const binaryString = window.atob(base64);
    const bytes = new Uint8Array(binaryString.length);
    for (let i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes.buffer;
}

function arrayBufferToBase64(buffer) {
    let binary = '';
    const bytes = new Uint8Array(buffer);
    for (let i = 0; i < bytes.byteLength; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}