import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.math.BigInteger;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.HashMap;
import java.util.Map;
import java.util.Scanner;
import java.util.regex.Pattern;

public class Main {

    public static byte[] getSHA(String input) throws NoSuchAlgorithmException
    {
        // Static getInstance method is called with hashing SHA
        MessageDigest md = MessageDigest.getInstance("SHA-256");

        // digest() method called
        // to calculate message digest of an input
        // and return array of byte
        return md.digest(input.getBytes(StandardCharsets.UTF_8));
    }

    public static String toHexString(byte[] hash)
    {
        // Convert byte array into signum representation
        BigInteger number = new BigInteger(1, hash);

        // Convert message digest into hex value
        StringBuilder hexString = new StringBuilder(number.toString(16));

        // Pad with leading zeros
        while (hexString.length() < 32)
        {
            hexString.insert(0, '0');
        }

        return hexString.toString();
    }

    public static void hashGenerator(int level, StringBuilder sb, Map<String, String> outputs){
        if(level == 3){
            try{
                outputs.put(sb.toString(), toHexString(getSHA(sb.toString())));
//                System.out.println(outputs.size());
            }catch (NoSuchAlgorithmException e){
                System.out.println("Issue with generating the SHA 256 Hash at line 45");
            }finally {
                return;
            }
        }
        for(int i = 32; i < 217; i++){
            sb.append((char)i);
            hashGenerator(level + 1, sb, outputs);
            sb.deleteCharAt(sb.length()-1);
        }
    }

    public static void main(String[] args) {
        Map<String, String> outputs = new HashMap<>();

        try {
            File hashes = new File("Hashes.txt");
            if (hashes.createNewFile()){
                System.out.println("New File Created: " + hashes.getName());
            }else {
                System.out.println("File Already Exists");
            }
        } catch (IOException e){
            System.out.println("Some Error has occured");
            e.printStackTrace();
        }

        try
        {
//            System.out.println("HashCode Generated by SHA-256 for:");
            FileWriter fileWriter = new FileWriter("Hashes.txt");
            StringBuilder sb = new StringBuilder("shreyansjain");
            hashGenerator(1, sb, outputs);
            for(Map.Entry set: outputs.entrySet()){
//                System.out.println(set.getKey() + "\t" + set.getValue());
                if(Pattern.matches("^00.*", (String)set.getValue())){
                    System.out.println(set.getKey() + "\t" + set.getValue() + "\n");
                    fileWriter.write(set.getKey() + "\t" + set.getValue() + "\n");
                }
//                String sha = toHexString(getSHA(sb.toString()));
//                System.out.println(sb.toString() + "\t" + sha);
//                fileWriter.write(sb.toString() + "\t" + sha + "\n");

            }
            fileWriter.close();
            // String s1 = "GeeksForGeeks";
            // System.out.println("\n" + s1 + " : " + toHexString(getSHA(s1)));

             String s2 = "hello world";
             System.out.println("\n" + s2 + " : " + toHexString(getSHA(s2)));
        }
        // For specifying wrong message digest algorithms
        catch (NoSuchAlgorithmException e) {
            System.out.println("Exception thrown for incorrect algorithm: " + e);
        } catch (IOException e){
            //Exception handler for fileWriter
            System.out.println("An error occurred with the fileWriter");
        }
        System.out.println(outputs.size());
        System.out.println("Program execution complete. :)");
    }
}
