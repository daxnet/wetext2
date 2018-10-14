package cn.sunnycoding.wetext.discoserver;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cloud.netflix.eureka.server.EnableEurekaServer;

@EnableEurekaServer
@SpringBootApplication
public class Wetext2DiscoServerApplication {

	public static void main(String[] args) {
		SpringApplication.run(Wetext2DiscoServerApplication.class, args);
	}
}
