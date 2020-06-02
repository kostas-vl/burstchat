/**
 * This interface contains properties that describe information about the access and refresh tokens
 * for the BurstChat identity server.
 * interface TokenInfo
 */
export interface TokenInfo {
    idToken: string | null;
    accessToken: string;
    expiresIn: number;
    refreshToken: string;
    scope: string;
    tokenType: string;
}
